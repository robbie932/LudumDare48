﻿using System.Collections;
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
    public float positionLerpFactor, rotationLerpFactor;
    public Transform cameraTransform;
    public AnimationCurve shakeCurve;
    public float sidewaysShakeFactor = 0.4f;


    private Quaternion rotation;
    private Vector3 offsetPosition;
    private float distance;
    private void OnValidate()
    {
        instance = this;

        rotation = Quaternion.Euler(verticalAngle, 0, 0);
        offsetPosition = offset + rotation * (-Vector3.forward * length);
        if (!Application.isPlaying)
        {
            transform.position = offsetPosition;
            transform.rotation = Quaternion.LookRotation(-(offsetPosition - offset).normalized);
        }
    }
    public void AddShake(float xForce, float yForce, float shakeDuration)
    {
        StopAllCoroutines();
        StartCoroutine(_Shake(xForce, yForce, shakeDuration));
    }

    private IEnumerator _Shake(float xForce, float yForce, float shakeDuration)
    {
        var t = 0f;
        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            var a = t / shakeDuration;
            var eval = shakeCurve.Evaluate(a);
            var y = Mathf.Lerp(0, yForce, eval);
            var point = Random.Range(-xForce, xForce);
            var x = Mathf.Lerp(cameraTransform.localPosition.x, point, sidewaysShakeFactor);

            cameraTransform.localPosition = new Vector3(x, y, 0);
            yield return null;
        }
    }

    private void UpdateRotation(float hRot)
    {
        rotation = Quaternion.Euler(verticalAngle, hRot, 0);
        offsetPosition = offset + rotation * (-Vector3.forward * length);
    }

    private void Awake()
    {
        instance = this;
        OnValidate();
    }

    public void ResetDistance()
    {
        distance = 0;
    }

    private void LateUpdate()
    {
        var point = PlatformCreator.instance.pathCreator.path.GetPointAtDistance(distance);
        var playerPos = Game.Player.transform.position;
        playerPos.y = point.y;//flat
        var dirToPlayer = playerPos - point;
        var a = dirToPlayer.z / 10f;
        var speed = Mathf.Lerp(0.5f, 2f, a);

        distance += Game.Player.maxSpeed * speed * Time.deltaTime;

        var t = point + offsetPosition;
        UpdateRotation(Game.Player.pathRotation.eulerAngles.y);
        transform.position = Vector3.Lerp(transform.position, t, positionLerpFactor);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationLerpFactor);


        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, Vector3.zero, 0.3f);
    }
}
