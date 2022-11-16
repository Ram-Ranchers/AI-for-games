using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CityGeneration
{
    public class Buildings : MonoBehaviour
    {
        public GameObject buildingContainer;
        public Terrain terrain;
        public List<Zones> zones;
        public List<BoxCollider> boxColliders;
        private GameObject[] gameObjectZone;
        
        private void Start()
        {
            if (gameObjectZone == null)
            {
                gameObjectZone = GameObject.FindGameObjectsWithTag("Zone");
            }

            zones = new List<Zones>();
            boxColliders = new List<BoxCollider>();
            
            for (int i = 0; i < gameObjectZone.Length; i++)
            {
                zones.Add(gameObjectZone[i].GetComponent<Zones>());
                boxColliders.Add(gameObjectZone[i].GetComponent<BoxCollider>());
            }
            
            SpawnBuildingsInsideZone();
        }
        
        void SpawnBuildingsInsideZone()
        {
            for (int i = 0; i < boxColliders.Count; i++)
            {
                for (int x = 0; x < boxColliders[i].bounds.size.x; x++)
                {
                    for (int z = 0; z < boxColliders[i].bounds.size.z; z++)
                    {
                        float width = Random.Range(1.75f, 2f);
                        float length = Random.Range(1.75f, 2f);
                        float height = Random.Range(2.5f, 10f);

                        float rotation = 0f;

                        Vector3 centre = new Vector3(x + boxColliders[i].transform.position.x - boxColliders[i].bounds.extents.x, 10.0f, 
                            z + boxColliders[i].transform.position.z - boxColliders[i].bounds.extents.x);

                        Vector3 size = new Vector3(length, width, height);

                        if (!Physics.CheckSphere(centre, width, 5, QueryTriggerInteraction.Ignore) &&
                            !Physics.CheckSphere(centre, length, 5, QueryTriggerInteraction.Ignore))
                        {
                            GameObject buildingObj = Instantiate(buildingContainer, centre, Quaternion.identity);

                            Building building = new Building(centre, size, rotation);
                            building.buildingType = zones[i].buildingType;
                            building.AddGameObject(buildingObj);
                            BuildingMesh(building);
                            building.AddCollider();
                            building.BuildingTypeParameters();
                        }
                    }
                }
            }
        }
        
        void SpawnBuildingsOnTerrain()
        {
            for (int x = 0; x < terrain.terrainData.bounds.size.x; x++)
            {
                for (int z = 0; z < terrain.terrainData.bounds.size.z; z++)
                {
                    float width = Random.Range(1.75f, 2f);
                    float length = Random.Range(1.75f, 2f);
                    float height = Random.Range(2.5f, 10f);
        
                    float rotation = 0f;
        
                    Vector3 centre = new Vector3(x, 10.0f, z);
        
                    Vector3 size = new Vector3(length, width, height);
        
                    if (!Physics.CheckSphere(centre, width, 5, QueryTriggerInteraction.Ignore) &&
                        !Physics.CheckSphere(centre, length, 5, QueryTriggerInteraction.Ignore))
                    {
                        GameObject buildingObj = Instantiate(buildingContainer, centre, Quaternion.identity);
        
                        Building building = new Building(centre, size, rotation);
                        building.AddGameObject(buildingObj);
                        BuildingMesh(building);
                        building.AddCollider();
                    }
                }
            }
        }

        private void BuildingMesh(Building building)
        {
            building.gameObject.GetComponent<MeshFilter>().mesh = new Mesh();

            Mesh mesh = building.gameObject.GetComponent<MeshFilter>().mesh;

            List<int> triangles = mesh.vertexCount == 0 ? new List<int>() : new List<int>(mesh.GetTriangles(0));
            List<Vector3> vertices = new List<Vector3>(mesh.vertices);
            List<Vector3> normals = new List<Vector3>(mesh.normals);
            if (normals == null)
            {
                throw new ArgumentNullException(nameof(normals));
            }

            List<Vector2> uvs = new List<Vector2>(mesh.uv);

            int lastTri = vertices.Count;

            Vector3 triLeft = new Vector3(+building.Size.x, +building.Size.z, -building.Size.y);
            Vector3 triRight = new Vector3(-building.Size.x, +building.Size.z, -building.Size.y);
            Vector3 bottomLeft = new Vector3(+building.Size.x, 0f, -building.Size.y);
            Vector3 bottomRight = new Vector3(-building.Size.x, 0f, -building.Size.y);

            GetQuad(new[] { triLeft, triRight, bottomLeft, bottomRight }, ref lastTri, out var tris,
                out var norms, out var uv);

            triangles.AddRange(tris);
            normals.AddRange(norms);
            vertices.AddRange(new[] { triLeft, triRight, bottomLeft, bottomRight });
            uvs.AddRange(uv);

            triLeft = new Vector3(+building.Size.x, +building.Size.z, +building.Size.y);
            triRight = new Vector3(+building.Size.x, +building.Size.z, -building.Size.y);
            bottomLeft = new Vector3(+building.Size.x, 0f, +building.Size.y);
            bottomRight = new Vector3(+building.Size.x, 0f, -building.Size.y);

            GetQuad(new[] { triLeft, triRight, bottomLeft, bottomRight }, ref lastTri, out tris,
                out norms, out uv);

            triangles.AddRange(tris);
            normals.AddRange(norms);
            vertices.AddRange(new[] { triLeft, triRight, bottomLeft, bottomRight });
            uvs.AddRange(uv);

            triLeft = new Vector3(-building.Size.x, +building.Size.z, -building.Size.y);
            triRight = new Vector3(-building.Size.x, +building.Size.z, +building.Size.y);
            bottomLeft = new Vector3(-building.Size.x, 0f, -building.Size.y);
            bottomRight = new Vector3(-building.Size.x, 0f, +building.Size.y);

            GetQuad(new[] { triLeft, triRight, bottomLeft, bottomRight }, ref lastTri, out tris,
                out norms, out uv);

            triangles.AddRange(tris);
            normals.AddRange(norms);
            vertices.AddRange(new[] { triLeft, triRight, bottomLeft, bottomRight });
            uvs.AddRange(uv);

            triLeft = new Vector3(-building.Size.x, +building.Size.z, +building.Size.y);
            triRight = new Vector3(+building.Size.x, +building.Size.z, +building.Size.y);
            bottomLeft = new Vector3(-building.Size.x, 0f, +building.Size.y);
            bottomRight = new Vector3(+building.Size.x, 0f, +building.Size.y);

            GetQuad(new[] { triLeft, triRight, bottomLeft, bottomRight }, ref lastTri, out tris,
                out norms, out uv);

            triangles.AddRange(tris);
            normals.AddRange(norms);
            vertices.AddRange(new[] { triLeft, triRight, bottomLeft, bottomRight });
            uvs.AddRange(uv);

            triLeft = new Vector3(+building.Size.x, +building.Size.z, +building.Size.y);
            triRight = new Vector3(-building.Size.x, +building.Size.z, +building.Size.y);
            bottomLeft = new Vector3(+building.Size.x, +building.Size.z, -building.Size.y);
            bottomRight = new Vector3(-building.Size.x, +building.Size.z, -building.Size.y);

            GetQuad(new[] { triLeft, triRight, bottomLeft, bottomRight }, ref lastTri, out tris,
                out norms, out uv);

            triangles.AddRange(tris);
            normals.AddRange(norms);
            vertices.AddRange(new[] { triLeft, triRight, bottomLeft, bottomRight });
            uvs.AddRange(uv);

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
        }

        private static void GetQuad(Vector3[] vertices, ref int last, out int[] triangles,
            out Vector3[] normals, out Vector2[] uvs)
        {
            triangles = new[] { last, last + 2, last + 1, last + 1, last + 2, last + 3 };
            last += 4;

            normals = new Vector3[] { };

            uvs = new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0) };
        }
    }

    public class Building
    {
        private Vector3 Center;

        public Vector3 Size;

        private float Rotation;

        private BoxCollider Collider;

        private Renderer renderer;

        public GameObject gameObject;

        public int buildingType;

        public Building(Vector3 center, Vector3 size, float rotation)
        {
            Center = new Vector3(center.x, center.y, center.z);
            Size = new Vector3(size.x, size.y, size.z);

            Rotation = rotation > 360 ? rotation - 360 : rotation;
            Rotation = rotation < 0 ? rotation + 360 : rotation;
        }

        public void AddGameObject(GameObject gameObject)
        {
            this.gameObject = gameObject;

            this.gameObject.transform.localPosition = Center;
            this.gameObject.transform.localRotation = Quaternion.Euler(0, Rotation, 0);
        }

        public void AddCollider()
        {
            Collider = gameObject.GetComponent<BoxCollider>();
            Collider.size = new Vector3(Size.x * 2, Size.z + 0.75f, Size.y * 2);
            Collider.center = new Vector3(0, Size.z / 2.0f, 0);
        }

        public void BuildingTypeParameters()
        {
            renderer = gameObject.GetComponent<Renderer>();

            switch (buildingType)
            {
                case 0:
                    renderer.material.SetColor("_Color", Color.green);
                    break;
                case 1:
                    renderer.material.SetColor("_Color", Color.blue);
                    break;
                case 2:
                    renderer.material.SetColor("_Color", Color.yellow);
                    break;
                default:
                    break;
            }
        }
    }
}