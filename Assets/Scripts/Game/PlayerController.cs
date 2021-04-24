using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class Game
{
    public static PlayerController Player => PlayerController.instance;
}

public class PlayerController : MonoBehaviour
{

    public static PlayerController instance;

    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f, maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;

    [SerializeField, Range(1f, 10f)]
    float fallMulltiplier = 2.5f;

    [SerializeField, Range(0f, 10f)]
    float lowJumpMulltiplier = 2.5f;

    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;

    [SerializeField]
    private float fallOffHelpTime = 0.1f;

    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    [SerializeField]
    private Rigidbody body;

    internal Vector3 pathPosition;
    internal Quaternion pathRotation;

    Vector3 velocity, desiredVelocity;

    Vector3 contactNormal;

    bool desiredJump;

    int groundContactCount;

    bool OnGround => groundContactCount > 0;
    bool onGroundLast;
    private float fallOffJumpValid;
    private bool IsFallTimeHelpValid => Time.time <= fallOffJumpValid && velocity.y < 0;

    int jumpPhase;

    float minGroundDotProduct;


    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void Awake()
    {
        instance = this;
        OnValidate();
    }


    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = 1;
        //playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        desiredVelocity =
            new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        desiredJump |= Input.GetButtonDown("Jump");

        UpdatePathData();

        if (transform.position.y < -50)
        {
            transform.position = Vector3.zero;
            velocity = Vector3.zero;
            body.AddForce(Vector3.zero, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }
        AdjustVerticalFallSpeed();

        body.velocity = velocity;
        ClearState();
    }

    public void UpdatePathData()
    {
        var path = Game.PlatformCreator.pathCreator.path;
        var closestDistOnPath = path.GetClosestDistanceAlongPath(transform.position);

        pathPosition = path.GetPointAtDistance(closestDistOnPath);
        pathRotation = path.GetRotationAtDistance(closestDistOnPath);
    }


    private void AdjustVerticalFallSpeed()
    {
        velocity += Physics.gravity * Time.fixedDeltaTime;
        if (velocity.y < 0)
        {
            velocity += Physics.gravity * ((fallMulltiplier - 1) * Time.fixedDeltaTime);
        }
        else if (velocity.y > 0 && !Input.GetButton("Jump"))
        {
            velocity += Physics.gravity * ((lowJumpMulltiplier - 1) * Time.fixedDeltaTime);
        }
    }

    void ClearState()
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
        onGroundLast = OnGround;
    }

    void UpdateState()
    {
        velocity = body.velocity;

        if (onGroundLast && !OnGround)
        {
            fallOffJumpValid = Time.time + fallOffHelpTime;
            //just fell
        }

        if (OnGround)
        {
            jumpPhase = 0;
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = Vector3.up;
        }
    }

    void AdjustVelocity()
    {
        var right = pathRotation * Vector3.right;
        var forward = pathRotation * Vector3.forward;

        Vector3 xAxis = ProjectOnContactPlane(right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX =
            Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    void Jump()
    {

        if (OnGround || IsFallTimeHelpValid || jumpPhase < maxAirJumps)
        {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            velocity += Vector3.up * jumpSpeed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct)
            {
                groundContactCount += 1;
                contactNormal += normal;
            }
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

}
