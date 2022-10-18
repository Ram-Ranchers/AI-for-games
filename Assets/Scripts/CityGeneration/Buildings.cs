using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Buildings : MonoBehaviour
{
    public GameObject buildingContainer;
    public GameObject instances;
    
    private List<Building> buildingsList { get; set; }


    public int gridX = 4;
    public int gridZ = 4;
    public Vector3 gridOrigin = Vector3.zero;
    public float gridOffset = 2f;
    public bool generateOnEnable;
    
    private void Start()
    {
        buildingsList = new List<Building>();
        if (generateOnEnable)
        {
            Generate();
        }
        //AddBuildings();
    }
    
    public void Generate()
    {
        SpawnGrid();
    }

    void SpawnGrid()
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                float width = Random.Range(1.75f, 2f);
                float length = Random.Range(1.75f, 2f);
                float height = Random.Range(2.5f, 10f);

                float rotation = 0f;
            
                Vector3 center = new Vector3(0.5f, 0, 0.5f);
            
                Vector3 size = new Vector3(length, width, height);

                GameObject buildingObj = Instantiate(buildingContainer, transform.position + gridOrigin + new Vector3(gridOffset * x, 0, gridOffset * z), transform.rotation);
                buildingObj.transform.name = "building_" + buildingsList.Count.ToString("D5");

                Building building = new Building(center, size, rotation);
                building.AddGameObject(buildingObj);
                BuildingMesh(building);
                building.AddCollider();

                if (CheckValidPlacement(building))
                {
                    buildingsList.Add(building);
                    break;
                }
            
                DestroyImmediate(buildingObj);
            }
        }
    }
    
    //private void AddBuildings()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        float width = Random.Range(1.75f, 2f);
    //        float length = Random.Range(1.75f, 2f);
    //        float height = Random.Range(2.5f, 10f);
//
    //        float rotation = 0f;
    //        
    //        Vector3 center = new Vector3(0.5f, 0, 0.5f);
    //        
    //        Vector3 size = new Vector3(length, width, height);
//
    //        GameObject buildingObj = Instantiate(buildingContainer, instances.transform, true);
    //        buildingObj.transform.name = "building_" + buildingsList.Count.ToString("D5");
//
    //        Building building = new Building(center, size, rotation);
    //        building.AddGameObject(buildingObj);
    //        BuildingMesh(building);
    //        building.AddCollider();
//
    //        if (CheckValidPlacement(building))
    //        {
    //            buildingsList.Add(building);
    //            break;
    //        }
    //        
    //        DestroyImmediate(buildingObj);
    //    }
    //}
    
    private void BuildingMesh(Building building)
    {
        building.GameObject.GetComponent<MeshFilter>().mesh = new Mesh();

        Mesh mesh = building.GameObject.GetComponent<MeshFilter>().mesh;

        List<int> triangles = mesh.vertexCount == 0 ? new List<int>() : 
            new List<int>(mesh.GetTriangles(0));
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
        
        GetQuad(new []{triLeft, triRight, bottomLeft, bottomRight}, ref lastTri, out var tris,
            out var norms, out var uv);
        
        triangles.AddRange(tris);
        normals.AddRange(norms);
        vertices.AddRange(new[]{triLeft, triRight, bottomLeft, bottomRight});
        uvs.AddRange(uv);
        
        triLeft = new Vector3(+building.Size.x, +building.Size.z, +building.Size.y);
        triRight = new Vector3(+building.Size.x, +building.Size.z, -building.Size.y);
        bottomLeft = new Vector3(+building.Size.x, 0f, +building.Size.y);
        bottomRight = new Vector3(+building.Size.x, 0f, -building.Size.y);
        
        GetQuad(new []{triLeft, triRight, bottomLeft, bottomRight}, ref lastTri, out tris,
            out norms, out uv);
        
        triangles.AddRange(tris);
        normals.AddRange(norms);
        vertices.AddRange(new[]{triLeft, triRight, bottomLeft, bottomRight});
        uvs.AddRange(uv);
        
        triLeft = new Vector3(-building.Size.x, +building.Size.z, -building.Size.y);
        triRight = new Vector3(-building.Size.x, +building.Size.z, +building.Size.y);
        bottomLeft = new Vector3(-building.Size.x, 0f, -building.Size.y);
        bottomRight = new Vector3(-building.Size.x, 0f, +building.Size.y);
        
        GetQuad(new []{triLeft, triRight, bottomLeft, bottomRight}, ref lastTri, out tris,
            out norms, out uv);
        
        triangles.AddRange(tris);
        normals.AddRange(norms);
        vertices.AddRange(new[]{triLeft, triRight, bottomLeft, bottomRight});
        uvs.AddRange(uv);
        
        triLeft = new Vector3(-building.Size.x, +building.Size.z, +building.Size.y);
        triRight = new Vector3(+building.Size.x, +building.Size.z, +building.Size.y);
        bottomLeft = new Vector3(-building.Size.x, 0f, +building.Size.y);
        bottomRight = new Vector3(+building.Size.x, 0f, +building.Size.y);
        
        GetQuad(new []{triLeft, triRight, bottomLeft, bottomRight}, ref lastTri, out tris,
            out norms, out uv);
        
        triangles.AddRange(tris);
        normals.AddRange(norms);
        vertices.AddRange(new[]{triLeft, triRight, bottomLeft, bottomRight});
        uvs.AddRange(uv);
        
        triLeft = new Vector3(+building.Size.x, +building.Size.z, +building.Size.y);
        triRight = new Vector3(-building.Size.x, +building.Size.z, +building.Size.y);
        bottomLeft = new Vector3(+building.Size.x, +building.Size.z, -building.Size.y);
        bottomRight = new Vector3(-building.Size.x, +building.Size.z, -building.Size.y);
        
        GetQuad(new []{triLeft, triRight, bottomLeft, bottomRight}, ref lastTri, out tris,
            out norms, out uv);
        
        triangles.AddRange(tris);
        normals.AddRange(norms);
        vertices.AddRange(new[]{triLeft, triRight, bottomLeft, bottomRight});
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

    private bool CheckValidPlacement(Building building)
    {
        foreach (var other in buildingsList)
        {
            if (Vector3.Distance(building.Center, other.Center) > 25f)
            {
                continue;
            }
        }

        return true;
    }
}

public class Building
{
    public Vector3 Center { get; }
    
    public Vector3 Size { get; }
    
    private float Rotation { get; }
    
    public BoxCollider Collider { get; private set; }
    
    public GameObject GameObject { get; private set; }

    public Building(Vector3 center, Vector3 size, float rotation)
    {
        Center = new Vector3(center.x, center.y, center.z);
        Size = new Vector3(size.x, size.y, size.z);

        Rotation = rotation > 360 ? rotation - 360 : rotation;
        Rotation = rotation < 0 ? rotation + 360 : rotation;
    }

    public void AddGameObject(GameObject gameObject)
    {
        GameObject = gameObject;

        GameObject.transform.localPosition = Center;
        GameObject.transform.localRotation = Quaternion.Euler(0, Rotation, 0);
    }

    public void AddCollider()
    {
        Collider = GameObject.GetComponent<BoxCollider>();
        Collider.size = new Vector3(Size.x * 2, Size.z + 0.75f, Size.y * 2);
        Collider.center = new Vector3(0, Size.z / 2.0f, 0);
    }
}
