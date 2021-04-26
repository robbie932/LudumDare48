using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFloater : MonoBehaviour
{
    public Vector3 RotationSpeeds;
    public bool RandomRotationSpeeds;
    public float RotSpeedMultiplier = 1.0f;

    public bool Bob;
    public float BobSpeed;
    public float FloatingHeight = 1.0f;

    float CurrentYPos_World;
    float SinVal;

    private void Start() {
        if (RandomRotationSpeeds) {
            int FlipX = GetRandomBoolAsInt();
            int FlipY = GetRandomBoolAsInt();
            int FlipZ = GetRandomBoolAsInt();

            RotationSpeeds = new Vector3(
                Random.Range(0.1f, 1f) * RotSpeedMultiplier * FlipX,
                Random.Range(0.1f, 1f) * RotSpeedMultiplier * FlipY,
                Random.Range(0.1f, 1f) * RotSpeedMultiplier * FlipZ) ;
        }
        CurrentYPos_World = this.transform.position.y;

        SinVal = Random.Range(-1f, 1f); //Start Y position set to random for first round.
        BobSpeed = BobSpeed*GetRandomBoolAsInt(); //To randomize up or down start direction
    }

    private int GetRandomBoolAsInt() {
        return (Random.value > 0.5f) ? -1 : 1;
    }

    void Update()
    {
        if (RotationSpeeds.magnitude > 0)
        {
            transform.Rotate(RotationSpeeds, Space.Self);
        }

        if (Bob) {
            
            transform.position = new Vector3(transform.position.x, CurrentYPos_World + (SinVal * FloatingHeight), transform.position.z);
            SinVal = Mathf.Sin(Time.time * BobSpeed );
        }
    }
}
