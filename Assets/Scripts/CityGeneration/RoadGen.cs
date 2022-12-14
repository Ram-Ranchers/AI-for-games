using System;
using UnchangedSplines;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CityGeneration
{
    public class RoadGen : MonoBehaviour
    {
        [SerializeField] private Terrain terrain;
        
        private Vector3 pointsPos;
        private GameObject road;
        private GameObject road2;
        private PathCreator pathCreator;
        private PathCreator pathCreator2;
        
        private void Awake()
        {
            RoadCreation();
            RoadGeneration();
        }

        private void OnEnable()
        {
            pathCreator.bezierPath.OnModified += OnPathModified;
        }

        private void OnDisable()
        {
            pathCreator.bezierPath.OnModified -= OnPathModified;
        }

        private void OnPathModified()
        {
            pathCreator.bezierPath = pathCreator.bezierPath;
            
            pathCreator.GetComponent<RoadMeshCreator>().TriggerUpdate();
        }
        
        // Add segments in the currently growing direction of the spline with a random deviation to grow from the sides and if there is an intersection
        // and it has the same priority make one end and let another carry on but if they are of a different priority let the highest priority carry on and and also make it so it has a higher chance of slitting off.
        private void RoadCreation()
        {
            road = new GameObject("RoadContainer");
            road.AddComponent<PathCreator>();
            road.AddComponent<RoadMeshCreator>();
            road.GetComponent<RoadMeshCreator>().roadMaterial = Resources.Load<Material>("Road");
            road.GetComponent<RoadMeshCreator>().undersideMaterial = Resources.Load<Material>("Road");
            road.GetComponent<RoadMeshCreator>().textureTiling = 100.0f;
            terrain = GameObject.Find("Terrain").GetComponent<Terrain>();

            // Create a new bezier path
            pathCreator = road.GetComponent<PathCreator>();
            pathCreator.bezierPath = new BezierPath(pointsPos, false, PathSpace.xz);
            pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;
            pathCreator.bezierPath.GlobalNormalsAngle = 0;
            
            // Make short segments and continue to add segments until the path reaches the end of the x and z axis of terrain
            while (pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1].x < terrain.terrainData.size.x &&
                   pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1].z < terrain.terrainData.size.z)
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
                road2 = new GameObject("RoadContainer");
                road2.AddComponent<PathCreator>();
                road2.AddComponent<RoadMeshCreator>();
                road2.GetComponent<RoadMeshCreator>().roadMaterial = Resources.Load<Material>("Road");
                road2.GetComponent<RoadMeshCreator>().undersideMaterial = Resources.Load<Material>("Road");
                road2.GetComponent<RoadMeshCreator>().textureTiling = 100.0f;
                terrain = GameObject.Find("Terrain").GetComponent<Terrain>();

                // Create a new bezier path
                pathCreator2 = road2.GetComponent<PathCreator>();
                pathCreator2.bezierPath = new BezierPath(pointsPos, false, PathSpace.xz);
                pathCreator2.bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;
                pathCreator2.bezierPath.GlobalNormalsAngle = 0;

                // Create a new bezier path, coming in from the side of the terrain
                pathCreator2.bezierPath.AddSegmentToEnd(new Vector3(
                    Random.Range(0.0f, terrain.terrainData.size.x),
                    terrain.SampleHeight(new Vector3(
                        Random.Range(0.0f, terrain.terrainData.size.x),
                        0,
                        Random.Range(0.0f, terrain.terrainData.size.z))),
                    Random.Range(0.0f, terrain.terrainData.size.z)));
                
                /*while (pathCreator2.bezierPath[pathCreator2.bezierPath.NumPoints - 1].x < terrain.terrainData.size.x &&
                       pathCreator2.bezierPath[pathCreator2.bezierPath.NumPoints - 1].z < terrain.terrainData.size.z)
                {
                    // Get the last point of the path
                    Vector3 lastPoint = pathCreator2.bezierPath[pathCreator2.bezierPath.NumPoints - 1];

                    // Get the y axis of each point and assign it to the sample height of the terrain
                    lastPoint.y = terrain.SampleHeight(lastPoint);

                    // Add a new segment to the path
                    pathCreator2.bezierPath.AddSegmentToEnd(new Vector3(
                        lastPoint.x + Random.Range(0.0f, 10.0f),
                        lastPoint.y,
                        lastPoint.z + Random.Range(0.0f, 10.0f)));
                }*/
                
                /*// Get the last point of the path
                Vector3 lastPoint = pathCreator2.bezierPath[pathCreator2.bezierPath.NumPoints - 1];

                // Get the y axis of each point and assign it to the sample height of the terrain
                lastPoint.y = terrain.SampleHeight(lastPoint);

                // Add a new segment to the path
                pathCreator2.bezierPath.AddSegmentToEnd(new Vector3(
                    lastPoint.x + Random.Range(0.0f, 10.0f),
                    lastPoint.y,
                    lastPoint.z + Random.Range(0.0f, 10.0f)));*/
                
                /*pathCreator2.bezierPath.SetPoint(0, new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10)));
                pathCreator2.bezierPath.SetPoint(1,
                    new Vector3(Random.Range(1, terrain.terrainData.size.x / 3), 0,
                        Random.Range(1, terrain.terrainData.size.z / 3)));
                pathCreator2.bezierPath.SetPoint(2,
                    new Vector3(Random.Range(2, terrain.terrainData.size.x / 2), 0,
                        Random.Range(2, terrain.terrainData.size.z / 2)));
                pathCreator2.bezierPath.SetPoint(3,
                    new Vector3(Random.Range(3, terrain.terrainData.size.x), 0,
                        Random.Range(3, terrain.terrainData.size.z)));*/

                /*if (pathCreator2.bezierPath.IsIntersecting(pathCreator2.bezierPath))
                {
                    pathCreator2.bezierPath.AddPath(pathCreator.bezierPath);
                }*/

                print("Anchor points : " + pathCreator2.bezierPath.NumAnchorPoints);
                print("Points : " + pathCreator2.bezierPath.NumPoints);
                print("Segments : " + pathCreator2.bezierPath.NumSegments);
            }
        }
    }
}