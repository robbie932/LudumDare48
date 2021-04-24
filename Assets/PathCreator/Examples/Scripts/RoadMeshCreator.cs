using System.Collections.Generic;
using System.Linq;
using PathCreation.Utility;
using UnityEngine;

namespace PathCreation.Examples
{
    public class RoadMeshCreator : PathSceneTool
    {
        [Header("Road settings")]
        public float roadWidth = .4f;
        [Range(0, .5f)]
        public float thickness = .15f;
        public bool flattenSurface;

        [Header("Material settings")]
        public Material top;
        public Material side;
        public Material front;
        public float textureTiling = 1;

        [SerializeField, HideInInspector]
        GameObject meshHolder;

        public PlatformData[] platforms = new PlatformData[1];

        [System.Serializable]
        public struct PlatformData
        {
            public float length, offset;
        }

        protected override void PathUpdated()
        {
            if (pathCreator != null)
            {
                CreateMultipleMeshes();
            }
        }

        private void CreateMultipleMeshes()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            for (int i = 0; i < platforms.Length; i++)
            {
                PlatformData platform = platforms[i];
                CreateRoadMesh(platform, i);
            }
        }

        void CreateRoadMesh(PlatformData data, int platformIndex)
        {
            var length = path.length;
            var distPerPoint = length / path.NumPoints;

            var pointOffset = Mathf.RoundToInt(data.offset / distPerPoint);
            var numPoints = Mathf.RoundToInt(data.length / distPerPoint);

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

            bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);
            Vector3 localUp;
            Vector3 localRight;
            for (int i = 0; i < numPoints; i++)
            {
                var pathIndexOffset = i + pointOffset;
                localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(pathIndexOffset), path.GetNormal(pathIndexOffset)) : path.up;
                localRight = (usePathNormals) ? path.GetNormal(pathIndexOffset) : Vector3.Cross(localUp, path.GetTangent(pathIndexOffset));

                // Find position to left and right of current path vertex
                Vector3 vertSideA = path.GetPoint(pathIndexOffset) - localRight * Mathf.Abs(roadWidth);
                Vector3 vertSideB = path.GetPoint(pathIndexOffset) + localRight * Mathf.Abs(roadWidth);

                // Add top of road vertices
                verts[vertIndex + 0] = vertSideA;
                verts[vertIndex + 1] = vertSideB;
                // Add bottom of road vertices
                verts[vertIndex + 2] = vertSideA - localUp * thickness;
                verts[vertIndex + 3] = vertSideB - localUp * thickness;

                // Duplicate vertices to get flat shading for sides of road
                verts[vertIndex + 4] = verts[vertIndex + 0];
                verts[vertIndex + 5] = verts[vertIndex + 1];
                verts[vertIndex + 6] = verts[vertIndex + 2];
                verts[vertIndex + 7] = verts[vertIndex + 3];

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
                uvs[vertIndex + 1] = new Vector2(1, path.times[i]);

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
                if (i < numPoints - 1 || path.isClosedLoop)
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

            localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(pointOffset), path.GetNormal(pointOffset)) : path.up;
            localRight = (usePathNormals) ? path.GetNormal(pointOffset) : Vector3.Cross(localUp, path.GetTangent(pointOffset));
            Vector3 localForward = Vector3.Cross(localUp, localRight);

            verts[vertIndex] = verts[0];
            verts[vertIndex + 1] = verts[1];
            verts[vertIndex + 2] = verts[2];
            verts[vertIndex + 3] = verts[3];

            normals[vertIndex] = localForward;
            normals[vertIndex + 1] = localForward;
            normals[vertIndex + 2] = localForward;
            normals[vertIndex + 3] = localForward;

            frontFaceTris[0] = vertIndex + 2;
            frontFaceTris[1] = vertIndex;
            frontFaceTris[2] = vertIndex + 1;

            frontFaceTris[3] = vertIndex + 2;
            frontFaceTris[4] = vertIndex + 1;
            frontFaceTris[5] = vertIndex + 3;


            var mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetNormals(normals);

            mesh.subMeshCount = 3;
            mesh.SetTriangles(underRoadTriangles.Concat(roadTriangles).ToArray(), 0);
            mesh.SetTriangles(sideOfRoadTriangles, 1);
            mesh.SetTriangles(frontFaceTris, 1);

            mesh.RecalculateBounds();
            //mesh.UploadMeshData(false);

            var go = new GameObject("Platform" + platformIndex.ToString());
            go.transform.parent = transform;
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterials = new[] { top, side, front };
            go.AddComponent<MeshCollider>().convex = true;
        }
    }
}