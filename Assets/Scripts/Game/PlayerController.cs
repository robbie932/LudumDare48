using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

partial class Game
{
    public static PlayerController Player => PlayerController.instance;
}

public partial class PlayerController : MonoBehaviour
{
    public string HERE;
    GameInput input;

    public static int Score;

    public static PlayerController instance;


    [Header("PhysX")]
    public float startingPathPosition;

    public float startingYPosition;

    [SerializeField, Range(0f, 200f)]
    public float maxSpeed = 10f;

    [SerializeField, Range(0f, 200f)]
    float maxAcceleration = 10f, maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;

    [SerializeField, Range(1f, 10f)]
    float fallMulltiplier = 2.5f;

    [SerializeField, Range(0f, 10f)]
    float lowJumpMulltiplier = 2.5f, jumpDecelerator = 1;

    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;

    [SerializeField]
    private float fallOffHelpTime = 0.1f;

    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    [SerializeField]
    private Rigidbody body;

    private RigidbodyConstraints onGround = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    private RigidbodyConstraints inAir = RigidbodyConstraints.FreezeRotation;

    Vector3 velocity, desiredVelocity;

    Vector3 contactNormal;

    bool desiredJump;

    int groundContactCount;

    bool OnGround => groundContactCount > 0;
    bool onGroundLast;
    private float fallOffJumpValid;
    private float lastY;

    private bool IsFallTimeHelpValid => Time.time <= fallOffJumpValid && velocity.y <= 0;

    int jumpPhase;

    float minGroundDotProduct;
    private bool holdingJump;
    private Vector3 startingPosition;

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void Awake()
    {
        instance = this;
        OnValidate();

        input = new GameInput();
        input.Player.Jump.started += ctx => OnJump();
        input.Player.Jump.canceled += ctx => OnReleaseJump();

        input.Player.Move.performed += ctx => OnMove(ctx.ReadValue<float>());
        input.Player.Move.canceled += ctx => OnStopMove();

        SetDesVel();
    }
    private void Start()
    {
        startingPosition = PlatformCreator.instance.pathCreator.path.GetPointAtDistance(startingPathPosition) + new Vector3(0, 1, 1);
        SetDebugStartPos();
        transform.position = startingPosition;
    }

    private void OnStopMove()
    {
        SetDesVel();
    }

    private void OnJump()
    {
        desiredJump |= true;
        holdingJump = true;
    }
    private void OnReleaseJump()
    {
        holdingJump = false;
    }
    private void OnMove(float axis)
    {
        SetDesVel(axis);
    }

    private void SetDesVel(float horizontal = 0)
    {
        desiredVelocity = new Vector3(horizontal, 0f, 1) * maxSpeed;
    }

    private void OnEnable()
    {
        input.Enable();
    }
    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        UpdateLeaningAnimations();
        transform.rotation = Quaternion.Euler(0, Game.Camera.curvePointRot.eulerAngles.y, 0);
        if (transform.position.y < lastY - 80)
        {
            transform.position = startingPosition;
            velocity = Vector3.zero;
            body.AddForce(Vector3.zero, ForceMode.VelocityChange);
            Game.Camera.ResetDistance();
            ResetToLevelStart();
        }
    }

    void ResetToLevelStart() {
        
        //Only for testing the track...
        if (startingPathPosition > 0) {
            SetDebugStartPos();
        }
        
        transform.position = startingPosition;
        velocity = Vector3.zero;
        Game.Camera.ResetDistance();
        body.AddForce(Vector3.zero, ForceMode.VelocityChange);
    }

    IEnumerator TinyPause() {
        yield return new WaitForFixedUpdate();
    }

    void SetDebugStartPos() {
        startingPosition = PlatformCreator.instance.pathCreator.path.GetPointAtDistance(startingPathPosition);
        startingPosition.y = startingPosition.y + startingYPosition;

        //Reset the rigidbody velocity..
        body.isKinematic = true;
        TinyPause();
        body.isKinematic = false;
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
        else
        {
            AdjustVerticalFallSpeed();
        }

        body.velocity = velocity;
        ClearState();
    }


    private void AdjustVerticalFallSpeed()
    {
        if (OnGround)
        {
            return;
        }
        if (velocity.y < 0)
        {
            velocity += Physics.gravity * ((fallMulltiplier - 1) * Time.fixedDeltaTime);
        }
        else if (velocity.y > 0)
        {
            if (!holdingJump)
            {
                velocity += Physics.gravity * ((lowJumpMulltiplier - 1) * Time.fixedDeltaTime);
            }
            else
            {
                velocity += Physics.gravity * ((jumpDecelerator - 1) * Time.fixedDeltaTime);
            }
        }
    }

    void ClearState()
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }

    void UpdateState()
    {
        velocity = body.velocity;

        if (onGroundLast && !OnGround)
        {
            fallOffJumpValid = Time.time + fallOffHelpTime;
            lastY = body.position.y;
            //just fell
        }
        else if (!onGroundLast && OnGround)
        {
            CameraController.instance.AddShake(0.2f, landShakeAmount, 0.3f);
            landingVfx.Play();
        }
        onGroundLast = OnGround;

        if (OnGround)
        {
            if (velocity.y <= 0)
            {
                body.constraints = onGround;
            }
            jumpPhase = 0;
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            body.constraints = inAir;
            contactNormal = Vector3.up;
        }
    }

    void AdjustVelocity()
    {
        var right = Game.Camera.curvePointRot * Vector3.right;
        var forward = Game.Camera.curvePointRot * Vector3.forward;

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

        velocity += xAxis * (newX - currentX);
        velocity += zAxis * (newZ - currentZ);
    }

    void Jump()
    {

        if (OnGround || IsFallTimeHelpValid || jumpPhase < maxAirJumps)
        {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            //float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            //if (alignedSpeed > 0f)
            //{
            //    jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            //}
            body.constraints = inAir;
            velocity.y = jumpSpeed;
            groundContactCount = 0;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
        }
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
