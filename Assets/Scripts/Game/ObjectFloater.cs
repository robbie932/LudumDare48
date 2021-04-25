using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFloater : MonoBehaviour
{

    public float RotationSpeed = 0.0f;
    public Vector3 StartRotation;
    public Vector3 RotationDirection; // Max 1, 1, 1
    public bool randomStartX;
    public int RandomStartX;
    public int RandomStartY;
    public int RandomStartZ;
    public bool DoRandomRotate;



    // Start is called before the first frame update
    void Start() {

        Vector3 AdditionalStartRotation = new Vector3(Random.Range(0, RotationSpeed * RandomStartX), Random.Range(0, RotationSpeed * RandomStartY), Random.Range(0, RotationSpeed * RandomStartZ));
        this.transform.Rotate(StartRotation+ AdditionalStartRotation, Space.World);


        if (DoRandomRotate) {
            RotationDirection = new Vector3(Random.Range(0, RotationSpeed), Random.Range(0, RotationSpeed), Random.Range(0, RotationSpeed));
        }
    }

    // Update is called once per frame
    void Update() {
        if (RotationSpeed > 0) {
            this.transform.Rotate(RotationDirection * RotationSpeed, Space.World);
        }
    }
}
