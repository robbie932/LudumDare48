using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;
using System.Linq;

partial class Game
{
    public static PlatformCreator PlatformCreator => PlatformCreator.instance;
}
public class PlatformCreator : PathSceneTool
{
    public static PlatformCreator instance;
    public static bool dirty;

    [Header("Get Settings")]
    public float[] Lengths = new float[3];
    public float[] Offsets = new float[3];
    public float[] SideOffsets = new float[3];
    public float GetLengths(PlatformData.SizeType type)
    {
        return (GetSetting(Lengths, type));
    }
    public float GetOffset(PlatformData.SizeType type)
    {
        return (GetSetting(Offsets, type));
    }
    public float GetSideOffset(PlatformData.SizeType type)
    {
        return (GetSetting(SideOffsets, type));
    }

    public float GetSetting(float[] from, PlatformData.SizeType type)
    {
        if (from.Length <= 0)
        {
            return 0;
        }
        return from[Mathf.Clamp((int)type, 0, from.Length - 1)];
    }

    [Header("Road settings")]
    public float roadWidth = .4f;
    [Range(0, 2f)]
    public float thickness = .15f;

    [Header("Material settings")]
    public Material mat;
    public float textureTiling = 1;

    [HideInInspector]
    public PlatformDataObject[] sections;

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        instance = this;
        if (dirty)
        {
            CreateMultipleMeshes();
            dirty = false;
        }
    }
    protected override void PathUpdated()
    {
        if (pathCreator != null)
        {
            CreateMultipleMeshes();
        }
    }

    [ContextMenu("Update")]
    public void CreateMultipleMeshes()
    {
        int numChildren = transform.childCount;
        for (int i = numChildren - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject, false);
        }

        var offset = 0f;
        var vOffset = 0f;
        for (int i = 0; i < sections.Length; i++)
        {
            if (sections[i] == null)
            {
                continue;
            }
            var platforms = sections[i].data;
            for (int j = 0; j < platforms.Length; j++)
            {
                PlatformData data = platforms[j];
                offset += data.Offset;

                var w = (data.sideCount - 1) * data.SideOffset;
                for (int s = 0; s < data.sideCount; s++)
                {
                    var pOffset = s * data.SideOffset - w * 0.5f;
                    CreateRoadMesh(offset, data.Length, j, pOffset, vOffset, 2);
                }
                offset += data.Length;
            }
            offset += sections[i].OffsetAfter;
            vOffset += sections[i].verticalOffset;
        }
    }

    void CreateRoadMesh(float offset, float length, int platformIndex, float positionOffset, float verticalOffset, int numPoints)
    {
        Vector3[] verts = new Vector3[numPoints * 8 + 4];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];

        int numTris = 2 * (numPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
        int[] frontFaceTris = new int[6];
        int[] roadTriangles = new int[numTris * 3];
        int[] underRoadTriangles = new int[numTris * 3];
        int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

        int vertIndex = 0;
        int triIndex = 0;

        // Vertices for the top of the road are layed out:
        // 0  1
        // 8  9
        // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
        int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
        int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

        for (int i = 0; i < numPoints; i++)
        {
            var a = (float)i / (numPoints - 1);

            var p = Vector3.Lerp(path.GetPointAtDistance(offset), path.GetPointAtDistance(offset + length), a);
            p.y = -verticalOffset;

            var pos = p;
            var rot = Quaternion.Lerp(path.GetRotationAtDistance(offset), path.GetRotationAtDistance(offset + length), a) * Quaternion.Euler(0, 0, 90);
            pos += rot * Vector3.right * positionOffset;

            var localUp = rot * Vector3.up;
            var localRight = rot * Vector3.right;
            //var localForward = rot * Vector3.forward;
            // Find position to left and right of current path vertex
            Vector3 pA = pos - localRight * Mathf.Abs(roadWidth);
            Vector3 pB = pos + localRight * Mathf.Abs(roadWidth);
            var pC = pA - localUp * thickness;
            var pD = pB - localUp * thickness;

            // Add top of road vertices
            verts[vertIndex + 0] = pA;
            verts[vertIndex + 1] = pB;
            // Add bottom of road vertices
            verts[vertIndex + 2] = pC;
            verts[vertIndex + 3] = pD;

            // Duplicate vertices to get flat shading for sides of road
            verts[vertIndex + 4] = verts[vertIndex + 0];
            verts[vertIndex + 5] = verts[vertIndex + 1];
            verts[vertIndex + 6] = verts[vertIndex + 2];
            verts[vertIndex + 7] = verts[vertIndex + 3];

            // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
            uvs[vertIndex + 0] = new Vector2(0, length * textureTiling * a);
            uvs[vertIndex + 1] = new Vector2(1, length * textureTiling * a);
            uvs[vertIndex + 2] = new Vector2(1, length * textureTiling * a);
            uvs[vertIndex + 3] = new Vector2(0, length * textureTiling * a);


            uvs[vertIndex + 4] = new Vector2(0, length * textureTiling * a);
            uvs[vertIndex + 5] = new Vector2(0, length * textureTiling * a);
            uvs[vertIndex + 6] = new Vector2(1, length * textureTiling * a);
            uvs[vertIndex + 7] = new Vector2(1, length * textureTiling * a);

            // Top of road normals
            normals[vertIndex + 0] = localUp;
            normals[vertIndex + 1] = localUp;
            // Bottom of road normals
            normals[vertIndex + 2] = -localUp;
            normals[vertIndex + 3] = -localUp;
            // Sides of road normals 
            normals[vertIndex + 4] = -localRight;
            normals[vertIndex + 5] = localRight;
            normals[vertIndex + 6] = -localRight;
            normals[vertIndex + 7] = localRight;

            // Set triangle indices
            if (i < numPoints - 1)
            {
                for (int j = 0; j < triangleMap.Length; j++)
                {
                    roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                    // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                    underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                }
                for (int j = 0; j < sidesTriangleMap.Length; j++)
                {
                    sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                }
            }

            vertIndex += 8;
            triIndex += 6;
        }

        var startRot = path.GetRotationAtDistance(offset);
        var localForward = startRot * Vector3.forward;

        verts[vertIndex] = verts[0];
        verts[vertIndex + 1] = verts[1];
        verts[vertIndex + 2] = verts[2];
        verts[vertIndex + 3] = verts[3];

        uvs[vertIndex] = new Vector2(0, 1);
        uvs[vertIndex + 1] = new Vector2(0, 0);
        uvs[vertIndex + 2] = new Vector2(1, 1);
        uvs[vertIndex + 3] = new Vector2(1, 0);

        normals[vertIndex] = -localForward;
        normals[vertIndex + 1] = -localForward;
        normals[vertIndex + 2] = -localForward;
        normals[vertIndex + 3] = -localForward;

        frontFaceTris[0] = vertIndex + 2;
        frontFaceTris[1] = vertIndex;
        frontFaceTris[2] = vertIndex + 1;

        frontFaceTris[3] = vertIndex + 2;
        frontFaceTris[4] = vertIndex + 1;
        frontFaceTris[5] = vertIndex + 3;

        var bounds = new Bounds(verts[0], Vector3.zero);
        for (int i = 1; i < verts.Length; i++)
        {
            Vector3 vert = verts[i];
            bounds.Encapsulate(vert);
        }
        var center = bounds.center;
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] -= center;
        }

        var mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);

        mesh.SetTriangles(underRoadTriangles.Concat(roadTriangles).Concat(sideOfRoadTriangles).Concat(frontFaceTris).ToArray(), 0);
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        var go = new GameObject("Platform" + platformIndex.ToString());
        go.layer = gameObject.layer;
        go.transform.parent = transform;
        go.transform.position = center;

        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        go.AddComponent<MeshCollider>().convex = true;
    }
    /*private void OnDrawGizmos()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            PlatformData data = platforms[i];

            var startPos = path.GetPointAtDistance(data.offset);
            var startRot = path.GetRotationAtDistance(data.offset);

            var endPos = path.GetPointAtDistance(data.offset + data.length);
            var endRot = path.GetRotationAtDistance(data.offset + data.length);
            CreatePlatform(startRot, startPos);
            CreatePlatform(endRot, endPos);

            void CreatePlatform(Quaternion rot, Vector3 pos)
            {
                var localUp = rot * Vector3.up;
                var localRight = rot * Vector3.right;

                Vector3 vertSideA = pos - localRight * Mathf.Abs(roadWidth);
                Vector3 vertSideB = pos + localRight * Mathf.Abs(roadWidth);


                Gizmos.DrawSphere(vertSideA, 0.5f);
                Gizmos.DrawSphere(vertSideB, 0.5f);
                Gizmos.DrawSphere(vertSideA - localUp * thickness, 0.5f);
                Gizmos.DrawSphere(vertSideB - localUp * thickness, 0.5f);
            }
        }
    }*/
}

/*
[ExecuteInEditMode]
public class PlatformCreator : PathSceneTool
{
    public GameObject prefab;
    public float spacing = 3;
    public float offset;
    public float partOffset = 3;
    public Vector3 globalOffset;
    const float minSpacing = .1f;

    public PlatformData[] platforms;

    [ContextMenu("GENERATE")]
    void Generate()
    {
        if (pathCreator != null && prefab != null)
        {
            DestroyObjects();

            VertexPath path = pathCreator.path;

            spacing = Mathf.Max(minSpacing, spacing);
            var dst = 0f;
            foreach (var item in platforms)
            {
                dst += item.offset;
                for (int i = 0; i < 3; i++)
                {
                    var a = (float)i / 2;
                    var centerRotation = path.GetRotationAtDistance(dst);
                    centerRotation = Quaternion.Euler(0, centerRotation.eulerAngles.y, 0);
                    var pOffset = Mathf.Lerp(-partOffset, partOffset, a) * (centerRotation * Vector3.right);
                    Vector3 point = path.GetPointAtDistance(dst) + pOffset + globalOffset;
                    Quaternion rot = centerRotation;
                    Instantiate(prefab, point, rot, transform);
                }
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
}*/
/*
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
                    var centerRotation = path.GetRotationAtDistance(dst);
                    var offsetRot = centerRotation * Quaternion.Euler(0, 0, partAngle);
                    var normal = path.GetNormalAtDistance(dst);
                    var dir = offsetRot * -normal * offset;
                    Vector3 point = path.GetPointAtDistance(dst) + dir + globalOffset;
                    Quaternion rot = offsetRot;
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
}*/