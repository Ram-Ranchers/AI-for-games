using System;
using System.Collections.Generic;
using BetterSpline;
using Unity.VisualScripting;
using UnityEngine;

namespace CityGeneration
{
    public class RoadGen : MonoBehaviour
    {
        [SerializeField] private Terrain terrain;
        [SerializeField] private List<Transform> waypoints;

        private GameObject obj;
        private int numOfPoints = 10;
        private Vector3 pointsPos;
        private Material roadTex;

        private void OnEnable()
        {
            
        }

        private void Start()
        {
            //obj = new GameObject("Road");
            //waypoints = new List<Transform>();
            //
            //for (int i = 0; i < numOfPoints; i++)
            //{
            //    waypoints.Add(Instantiate(obj,
            //        new Vector3(Random.Range(0, terrain.terrainData.bounds.size.x), 0,
            //            Random.Range(0, terrain.terrainData.bounds.size.z)),
            //        Quaternion.identity).transform);
            //}
            //
            //if (waypoints.Count > 0)
            //{
            //    BezierPath bezierPath = new BezierPath(waypoints);
            //    GetComponent<PathCreator>().bezierPath = bezierPath;
            //    bezierPath.GetPointsInSegment(bezierPath.NumSegments);
            //    print(bezierPath.NumSegments);
            //    //obj.AddComponent<RoadMeshCreator>();
            //}
            
            gameObject.AddComponent<PathCreator>();
            gameObject.AddComponent<RoadMeshCreator>();
            GetComponent<RoadMeshCreator>().roadMaterial = Resources.Load<Material>("Road");
            GetComponent<RoadMeshCreator>().undersideMaterial = Resources.Load<Material>("Road");
            GetComponent<RoadMeshCreator>().textureTiling = 100.0f;
            terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
            Vector3 newPoint = Utility.TransformPoint(pointsPos, transform);
            BezierPath bezierPath = new BezierPath(newPoint);
            //VertexPath vertexPath = new VertexPath(bezierPath, newPoint);
            GetComponent<PathCreator>().bezierPath = bezierPath;
            bezierPath.SetPoint(0, new Vector3(0, 0, 0));
            bezierPath.SetPoint(1, new Vector3(terrain.terrainData.size.x / 3, 0, terrain.terrainData.bounds.size.z / 3));
            bezierPath.SetPoint(2, new Vector3(terrain.terrainData.size.x / 2, 0, terrain.terrainData.bounds.size.z / 2));
            bezierPath.SetPoint(3, new Vector3(terrain.terrainData.size.x, 0, terrain.terrainData.bounds.size.z));
            //bezierPath.AddSegmentToStart(newPoint);
            //bezierPath.AddSegmentToEnd(newPoint);
            //bezierPath.GetPointsInSegment(bezierPath.NumPoints);
            //Instantiate(obj, Vector3.zero, Quaternion.identity);
            //bezierPath.AddSegmentToStart(terrain.terrainData.size);
            print("Anchor points : " + bezierPath.NumAnchorPoints);
            print("Points : " + bezierPath.NumPoints);
            print("Segments : " + bezierPath.NumSegments);
        }
    }
}
