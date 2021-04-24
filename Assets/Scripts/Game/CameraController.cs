using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class Game
{
    public static CameraController Camera => CameraController.instance;
}

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Vector3 offset;
    public float verticalAngle = 30;
    public float length = 10;
    public float positionLerpFactor;

    private Quaternion rotation;
    private Vector3 offsetPosition;

    private void OnValidate()
    {
        rotation = Quaternion.Euler(verticalAngle, 0, 0);
        offsetPosition = offset + rotation * (-Vector3.forward * length);

        if (!Application.isPlaying)
        {
            transform.position = offsetPosition;
            transform.rotation = Quaternion.LookRotation(-(offsetPosition-offset).normalized);
        }
    }

    private void Awake()
    {
        instance = this;
        OnValidate();
    }

    private void LateUpdate()
    {
        var t = Game.Player.transform.position + offsetPosition;
        transform.position = Vector3.Lerp(transform.position, t, positionLerpFactor);
    }
}
