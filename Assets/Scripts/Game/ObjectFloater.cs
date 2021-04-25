using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFloater : MonoBehaviour
{

    public float RotationSpeed = 0.1f;

    public float dir1;
    public float dir2;
    public float dir3;
    Vector3 RotationDirection;
    // Start is called before the first frame update
    void Start()
    {
        dir1 = Random.Range(0, RotationSpeed);
        dir2 = Random.Range(0, RotationSpeed);
        dir3 = Random.Range(0, RotationSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(dir1, dir2, dir3, Space.World);
    }
}
