using UnchangedSplines;
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
            RoadGeneration();
        }

        private void RoadGeneration()
        {
            for (int i = 0; i < 10; i++)
            {
                road = new GameObject("RoadContainer");
                road.AddComponent<PathCreator>();
                road.AddComponent<RoadMeshCreator>();
                road.AddComponent<ObjectPlacer>();
                road.GetComponent<RoadMeshCreator>().roadMaterial = Resources.Load<Material>("Road");
                road.GetComponent<RoadMeshCreator>().undersideMaterial = Resources.Load<Material>("Road");
                road.GetComponent<RoadMeshCreator>().textureTiling = 100.0f;

                terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
                Vector3 newPoint = MathUtility.TransformPoint(pointsPos, transform, PathSpace.xz);
                BezierPath bezierPath = new BezierPath(newPoint);
                road.GetComponent<PathCreator>().bezierPath = bezierPath;
                bezierPath.SetPoint(0, new Vector3(Random.Range(0, 10), 8, Random.Range(0, 10)));
                bezierPath.SetPoint(1,
                    new Vector3(Random.Range(1, terrain.terrainData.size.x / 3), 8, Random.Range(1, terrain.terrainData.size.z / 3)));
                bezierPath.SetPoint(2,
                    new Vector3(Random.Range(2, terrain.terrainData.size.x / 2), 8, Random.Range(2, terrain.terrainData.size.z / 2)));
                bezierPath.SetPoint(3, new Vector3(Random.Range(3, terrain.terrainData.size.x), 8, Random.Range(3, terrain.terrainData.size.z)));
               
                print("Anchor points : " + bezierPath.NumAnchorPoints);
                print("Points : " + bezierPath.NumPoints);
                print("Segments : " + bezierPath.NumSegments);
            }
        }
    }
}
