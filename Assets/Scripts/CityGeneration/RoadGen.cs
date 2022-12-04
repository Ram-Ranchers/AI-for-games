using BetterSpline;
using UnityEngine;

namespace CityGeneration
{
    public class RoadGen : MonoBehaviour
    {
        [SerializeField] private Terrain terrain;
        [SerializeField] private GameObject road;
        
        private GameObject obj;
        private int numOfPoints = 10;
        private Vector3 pointsPos;
        private Material roadTex;

        private void Start()
        {
            road = GameObject.Find("RoadContainer");
            road.AddComponent<PathCreator>();
            road.AddComponent<RoadMeshCreator>();
            road.GetComponent<RoadMeshCreator>().roadMaterial = Resources.Load<Material>("Road");
            road.GetComponent<RoadMeshCreator>().undersideMaterial = Resources.Load<Material>("Road");
            road.GetComponent<RoadMeshCreator>().textureTiling = 100.0f;
            //Instantiate(road, Vector3.zero, Quaternion.identity);
            //Instantiate(road, Vector3.zero, Quaternion.identity);
            //Instantiate(road, Vector3.zero, Quaternion.identity);
            terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
            Vector3 newPoint = Utility.TransformPoint(pointsPos, transform);
            BezierPath bezierPath = new BezierPath(newPoint);
            road.GetComponent<PathCreator>().bezierPath = bezierPath;
            bezierPath.SetPoint(0, new Vector3(0, 0, 0));
            bezierPath.SetPoint(1, new Vector3(terrain.terrainData.size.x / 3, 0, terrain.terrainData.bounds.size.z / 3));
            bezierPath.SetPoint(2, new Vector3(terrain.terrainData.size.x / 2, 0, terrain.terrainData.bounds.size.z / 2));
            bezierPath.SetPoint(3, new Vector3(terrain.terrainData.size.x, 0, terrain.terrainData.bounds.size.z));
            for (int i = 0; i < 10; i++)
            {
                bezierPath.SplitSegment(new Vector3(terrain.terrainData.size.x / 2 + i, 0, terrain.terrainData.bounds.size.z / 2 + i), 0, 1f);
            }
            //bezierPath.AddSegmentToStart(newPoint);
            //bezierPath.AddSegmentToEnd(newPoint);
            //bezierPath.SetPoint(4, new Vector3(terrain.terrainData.size.x - 6, 0, terrain.terrainData.bounds.size.z - 6));
            //bezierPath.GetPointsInSegment(bezierPath.NumPoints);
            //Instantiate(obj, Vector3.zero, Quaternion.identity);
            //bezierPath.AddSegmentToStart(terrain.terrainData.size);
            print("Anchor points : " + bezierPath.NumAnchorPoints);
            print("Points : " + bezierPath.NumPoints);
            print("Segments : " + bezierPath.NumSegments);
        }
    }
}
