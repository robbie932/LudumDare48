using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class PlayerController
{
    [Header("Animation")]
    public Animator animator;
    public Transform model;
    public float maxRotation = 10f;
    public float forwardLean = 10f;
    public float leanSpeed = 0.6f;

    private float leanAmount;
    private void UpdateLeaningAnimations()
    {
        var normalizedX = desiredVelocity.x / maxSpeed;
        var normalizedZVel = velocity.z / maxSpeed;
        var a = (normalizedX * 0.5f) + 0.5f;
        leanAmount = Mathf.Lerp(leanAmount, a, leanSpeed);
        animator.SetFloat("Lean", leanAmount);
        animator.SetBool("OnGround", OnGround);
        var t = Quaternion.Euler(normalizedZVel * forwardLean, 0, normalizedX * -maxRotation);
        model.localRotation = Quaternion.Lerp(model.localRotation, t, leanSpeed);
    }
}
