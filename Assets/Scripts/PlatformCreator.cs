using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

[ExecuteInEditMode]
public class PlatformCreator : PathSceneTool
{
    public GameObject prefab;
    public float spacing = 3;
    public float offset;
    public float angle = 30;
    public Vector3 globalOffset;
    const float minSpacing = .1f;

    void Generate()
    {
        if (pathCreator != null && prefab != null)
        {
            DestroyObjects();

            VertexPath path = pathCreator.path;

            spacing = Mathf.Max(minSpacing, spacing);
            float dst = 0;

            while (dst < path.length)
            {
                for (int i = 0; i < 3; i++)
                {
                    var a = (float)i / 2;
                    var partAngle = Mathf.Lerp(-angle, angle, a);
                    var offsetRot = Quaternion.Euler(0, 0, partAngle);
                    var dir = offsetRot * Vector3.down * offset;

                    Vector3 point = path.GetPointAtDistance(dst) + dir + globalOffset;
                    Quaternion rot = offsetRot * path.GetRotationAtDistance(dst);
                    Instantiate(prefab, point, rot, transform);
                }
                dst += spacing;
            }
        }
    }

    void DestroyObjects()
    {
        int numChildren = transform.childCount;
        for (int i = numChildren - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject, false);
        }
    }

    protected override void PathUpdated()
    {
        if (pathCreator != null)
        {
            Generate();
        }
    }
}