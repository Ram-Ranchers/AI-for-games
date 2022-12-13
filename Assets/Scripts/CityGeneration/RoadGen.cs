using UnchangedSplines;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CityGeneration
{
    public class RoadGen : MonoBehaviour
    {
        [SerializeField] private Terrain terrain;
        
        private Vector3 pointsPos;

        private void Awake()
        {
            RoadCreation();
        }
        
        // Add segments in the currently growing direction of the spline with a random deviation to grow from the sides and if there is an intersection
        // and it has the same priority make one end and let another carry on but if they are of a different priority let the highest priority carry on and and also make it so it has a higher chance of slitting off.
        private void RoadCreation()
        {
            GameObject road = new GameObject("RoadContainer");
            road.AddComponent<PathCreator>();
            road.AddComponent<RoadMeshCreator>();
            road.GetComponent<RoadMeshCreator>().roadMaterial = Resources.Load<Material>("Road");
            road.GetComponent<RoadMeshCreator>().undersideMaterial = Resources.Load<Material>("Road");
            road.GetComponent<RoadMeshCreator>().textureTiling = 100.0f;
            terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
            
            // Create a new bezier path
            PathCreator pathCreator = road.GetComponent<PathCreator>();
            pathCreator.bezierPath = new BezierPath(pointsPos, false, PathSpace.xz);
            pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;
            pathCreator.bezierPath.GlobalNormalsAngle = 0;
            
            // Make short segments and continue to add segments until the path reaches the end of the x and z axis of terrain
            while (pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1].x < terrain.terrainData.size.x && pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1].z < terrain.terrainData.size.z)
            {
                // Get the last point of the path
                Vector3 lastPoint = pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1];
                
                // Get the y axis of each point and assign it to the sample height of the terrain
                lastPoint.y = terrain.SampleHeight(lastPoint);

                // Add a new segment to the path
                pathCreator.bezierPath.AddSegmentToEnd(new Vector3(
                    lastPoint.x + Random.Range(0.0f, 10.0f),
                    lastPoint.y,
                    lastPoint.z + Random.Range(0.0f, 10.0f)));
            }
        }

        private void RoadGeneration()
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject road = new GameObject("RoadContainer");
                road.AddComponent<PathCreator>();
                road.AddComponent<RoadMeshCreator>();
                road.AddComponent<ObjectPlacer>();
                road.GetComponent<RoadMeshCreator>().roadMaterial = Resources.Load<Material>("Road");
                road.GetComponent<RoadMeshCreator>().undersideMaterial = Resources.Load<Material>("Road");
                road.GetComponent<RoadMeshCreator>().textureTiling = 100.0f;
                terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
                
                BezierPath bezierPath = new BezierPath(pointsPos, false, PathSpace.xz);
                road.GetComponent<PathCreator>().bezierPath = bezierPath;
                pointsPos.y = terrain.SampleHeight(pointsPos);
                
                bezierPath.SetPoint(0, new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10)));
                bezierPath.SetPoint(1,
                    new Vector3(Random.Range(1, terrain.terrainData.size.x / 3), 0,
                        Random.Range(1, terrain.terrainData.size.z / 3)));
                bezierPath.SetPoint(2,
                    new Vector3(Random.Range(2, terrain.terrainData.size.x / 2), 0,
                        Random.Range(2, terrain.terrainData.size.z / 2)));
                bezierPath.SetPoint(3,
                    new Vector3(Random.Range(3, terrain.terrainData.size.x), 0,
                        Random.Range(3, terrain.terrainData.size.z)));

                bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;

                if (bezierPath.IsIntersecting(bezierPath))
                {
                    bezierPath.AddPath(new BezierPath(pointsPos, false, PathSpace.xz));
                }
                
                print("Anchor points : " + bezierPath.NumAnchorPoints);
                print("Points : " + bezierPath.NumPoints);
                print("Segments : " + bezierPath.NumSegments);
            }
        }
    }
}
