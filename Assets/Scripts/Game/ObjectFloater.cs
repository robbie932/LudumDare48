using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFloater : MonoBehaviour
{

    public Vector3 RotationSpeed;
    public Vector3 StartRotation;
    public Vector3 RotationDirection; // Max 1, 1, 1
    public int RandomStartX;
    public int RandomStartY;
    public int RandomStartZ;
     bool RandomDirFlip;
    public bool DoRandomRotate;

    public Vector3 FlippedAxis;
    // Start is called before the first frame update
    void Start() {
        if (RandomDirFlip) {
            FlippedAxis = new Vector3(-1, -1, -1);
        }

        Vector3 AdditionalStartRotation = new Vector3(Random.Range(0, 360 * RandomStartX), Random.Range(0, 360 * RandomStartY), Random.Range(0, 360 * RandomStartZ));
        this.transform.Rotate(StartRotation+ AdditionalStartRotation, Space.World);

        

        if (DoRandomRotate) {
            RotationDirection = Random.insideUnitCircle.normalized; // new Vector3(Random.Range(-RotationSpeed, RotationSpeed), Random.Range(-RotationSpeed, RotationSpeed), Random.Range(-RotationSpeed, RotationSpeed));
        }

        if (RandomDirFlip) {
            int FlipX = Random.Range(0, 1);
            int FlipY = Random.Range(0, 1);
            int FlipZ = Random.Range(0, 1);

            FlippedAxis = new Vector3(-1+(2* FlipX), -1 + (2 * FlipY), -1 + (2 * FlipZ));
        }

    }

    // Update is called once per frame
    void Update() {
        if (RotationSpeed.magnitude > 0) {
            
            transform.Rotate( RotationSpeed , Space.Self);
        }
    }
}
