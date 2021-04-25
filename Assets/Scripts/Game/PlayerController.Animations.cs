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
    public float rollChance = 0.1f;
    private float leanAmount;
    private void UpdateLeaningAnimations()
    {
        var normalizedX = desiredVelocity.x / maxSpeed;
        var normalizedZVel = velocity.z / maxSpeed;
        var a = (normalizedX * 0.5f) + 0.5f;
        leanAmount = Mathf.Lerp(leanAmount, a, leanSpeed);
        animator.SetFloat("Lean", leanAmount);
        animator.SetBool("OnGround", OnGround);
        animator.SetBool("ShouldRoll", Random.value >= (1 - rollChance));
        var t = Quaternion.Euler(normalizedZVel * forwardLean, 0, normalizedX * -maxRotation);
        model.localRotation = Quaternion.Lerp(model.localRotation, t, leanSpeed);
    }
}
#if UNITY_EDITOR
public static class ALIGN
{
    // Add a menu item to create custom GameObjects.
    // Priority 1 ensures it is grouped with the other menu items of the same kind
    // and propagated to the hierarchy dropdown and hierarchy context menus.
    [UnityEditor.MenuItem("GameObject/Align/Space Out", false, 10)]
    static void SpaceOut(UnityEditor.MenuCommand menuCommand)
    {
        var tra = UnityEditor.Selection.transforms;

        for (int i = 0; i < tra.Length; i++)
        {
            tra[i].position = Vector3.right * i * 20f;
        }
    }
}
#endif