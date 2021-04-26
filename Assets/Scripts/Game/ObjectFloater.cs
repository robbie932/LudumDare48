using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFloater : MonoBehaviour
{
    public Vector3 RotationSpeeds;
    public bool RandomRotationSpeeds;
    public float RotSpeedMultiplier = 1.0f;

    public Vector3 scaleDir;
    public float scaleMovement;
    public float scaleSpeed = 1;
    public AnimationCurve scaleCurve;

    public bool Bob;
    public float BobSpeed;
    public float FloatingHeight = 1.0f;

    private Vector3 startScale;
    float CurrentYPos_World;
    float SinVal;

    private void Start()
    {
        startScale = transform.localScale;
        if (RandomRotationSpeeds)
        {
            int FlipX = GetRandomBoolAsInt(); print(FlipX);
            int FlipY = GetRandomBoolAsInt();
            int FlipZ = GetRandomBoolAsInt();

            RotationSpeeds = new Vector3(
                Random.Range(0.1f, 1f) * RotSpeedMultiplier * FlipX,
                Random.Range(0.1f, 1f) * RotSpeedMultiplier * FlipY,
                Random.Range(0.1f, 1f) * RotSpeedMultiplier * FlipZ);
        }
        CurrentYPos_World = this.transform.position.y;

        SinVal = Random.Range(-1f, 1f); //Start Y position set to random for first round.
        BobSpeed = BobSpeed * GetRandomBoolAsInt(); //To randomize up or down start direction
    }

    private int GetRandomBoolAsInt()
    {
        return (Random.value > 0.5f) ? -1 : 1;
    }

    void Update()
    {
        if (RotationSpeeds.magnitude > 0)
        {
            transform.Rotate(RotationSpeeds, Space.Self);
        }

        if (Bob)
        {

            transform.position = new Vector3(transform.position.x, CurrentYPos_World + (SinVal * FloatingHeight), transform.position.z);
            SinVal = Mathf.Sin(Time.time * BobSpeed);
        }

        if (scaleSpeed == 0)
        {
            return;
        }
        var a = (Time.time * scaleSpeed ) % 1f;
        transform.localScale = startScale + scaleDir * (scaleMovement * scaleCurve.Evaluate(a));
    }
}
