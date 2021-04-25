using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFloater : MonoBehaviour
{
    public Vector3 RotationSpeed;

    // Update is called once per frame
    void Update()
    {
        if (RotationSpeed.magnitude > 0)
        {
            transform.Rotate(RotationSpeed, Space.Self);
        }
    }
}
