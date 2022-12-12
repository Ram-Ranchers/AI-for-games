using System;
using UnityEngine;

namespace UnchangedSplines 
{
    public class RoadMeshCreator : PathSceneTool 
    {
        [Header ("Road settings")]
        public float roadWidth = .4f;
        [Range (0, .5f)]
        public float thickness = .15f;

        [Header ("Material settings")]
        public Material roadMaterial;
        public Material undersideMaterial;
        public float textureTiling = 1;
        
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh mesh;
        private MeshCollider meshCollider;
        
        protected override void PathUpdated() 
        {
            if (pathCreator != null)
            {
                AssignMeshComponents();
                AssignMaterials();
                CreateRoadMesh();
            }
        }
        
        private void CreateRoadMesh() 
        {
            Vector3[] verts = new Vector3[path.NumPoints * 8];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1);
            int[] roadTriangles = new int[numTris * 3];
            int[] underRoadTriangles = new int[numTris * 3];
            int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

            int vertIndex = 0;
            int triIndex = 0;

            int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
            int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };
            
            for (int i = 0; i < path.NumPoints; i++) 
            {
                Vector3 localUp = path.up;
                Vector3 localRight = Vector3.Cross(localUp, path.GetTangent(i));
                
                Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth);
                Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth);
                
                verts[vertIndex + 0] = vertSideA;
                verts[vertIndex + 1] = vertSideB;
                
                verts[vertIndex + 2] = vertSideA - localUp * thickness;
                verts[vertIndex + 3] = vertSideB - localUp * thickness;

                verts[vertIndex + 4] = verts[vertIndex + 0];
                verts[vertIndex + 5] = verts[vertIndex + 1];
                verts[vertIndex + 6] = verts[vertIndex + 2];
                verts[vertIndex + 7] = verts[vertIndex + 3];

                uvs[vertIndex + 0] = new Vector2 (0, path.times[i]);
                uvs[vertIndex + 1] = new Vector2 (1, path.times[i]);

                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;

                normals[vertIndex + 2] = -localUp;
                normals[vertIndex + 3] = -localUp;

                normals[vertIndex + 4] = -localRight;
                normals[vertIndex + 5] = localRight;
                normals[vertIndex + 6] = -localRight;
                normals[vertIndex + 7] = localRight;

                if (i < path.NumPoints - 1) 
                {
                    for (int j = 0; j < triangleMap.Length; j++) 
                    {
                        roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
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

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.subMeshCount = 3;
            mesh.SetTriangles(roadTriangles, 0);
            mesh.SetTriangles(underRoadTriangles, 1);
            mesh.SetTriangles(sideOfRoadTriangles, 2);
            mesh.RecalculateBounds();
        }
        
        private void AssignMeshComponents() 
        {
            transform.rotation = Quaternion.identity;
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            
            if (!gameObject.GetComponent<MeshFilter>()) 
            {
                gameObject.AddComponent<MeshFilter>();
            }
            if (!GetComponent<MeshRenderer>()) 
            {
                gameObject.AddComponent<MeshRenderer>();
            }
            if (!GetComponent<MeshCollider>())
            {
                gameObject.AddComponent<MeshCollider>();
            }
            
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
            
            if (mesh == null) 
            {
                mesh = new Mesh ();
            }
            
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        private void AssignMaterials() 
        {
            if (roadMaterial != null && undersideMaterial != null) 
            {
                meshRenderer.sharedMaterials = new[] { roadMaterial, undersideMaterial, undersideMaterial };
                meshRenderer.sharedMaterials[0].mainTextureScale = new Vector3 (1, textureTiling);
            }
        }
    }
}