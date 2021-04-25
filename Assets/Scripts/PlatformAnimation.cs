using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAnimation : MonoBehaviour
{
    public MeshRenderer rend;
    public float duration = 0.5f;
    public AnimationCurve curve;

    private void OnCollisionEnter(Collision collision)
    {
        StopAllCoroutines();
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        var t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            var a = t / duration;
            var eval = curve.Evaluate(a);
            rend.material.SetFloat("Vector1_7C879C9E", eval);
            yield return null;
        }
    }
}
