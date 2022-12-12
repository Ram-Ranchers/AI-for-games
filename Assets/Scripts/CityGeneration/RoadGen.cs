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
            RoadGeneration();
            //RoadCreation();
        }
        
        // Create a road network with splines
        
        
        // Create new gameobject for each new bezier path with a road mesh creator component and Set all the points
        private void RoadCreation()
        {
            GameObject road = new GameObject("RoadContainer");
            road.AddComponent<PathCreator>();
            road.AddComponent<RoadMeshCreator>();
            road.AddComponent<ObjectPlacer>();
            road.GetComponent<RoadMeshCreator>().roadMaterial = Resources.Load<Material>("Road");
            road.GetComponent<RoadMeshCreator>().undersideMaterial = Resources.Load<Material>("Road");
            road.GetComponent<RoadMeshCreator>().textureTiling = 100.0f;
            terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
            
            // Create a new bezier path
            PathCreator pathCreator = road.GetComponent<PathCreator>();
            pathCreator.bezierPath = new BezierPath(pointsPos, false, PathSpace.xz);
            pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;
            pathCreator.bezierPath.GlobalNormalsAngle = 0;
            
            var points = new Vector3[4];
            for (var i = 0; i < points.Length; i++)
            {
                points[i] = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));
                pointsPos = points[i];
            }
        }

        private void RoadGeneration()
        {
            /*GameObject road = new GameObject("RoadContainer");
            road.AddComponent<PathCreator>();
            road.AddComponent<RoadMeshCreator>();
            road.AddComponent<ObjectPlacer>();
            road.GetComponent<RoadMeshCreator>().roadMaterial = Resources.Load<Material>("Road");
            road.GetComponent<RoadMeshCreator>().undersideMaterial = Resources.Load<Material>("Road");
            road.GetComponent<RoadMeshCreator>().textureTiling = 100.0f;
            terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
            //Vector3 newPoint = MathUtility.TransformPoint(pointsPos, transform, PathSpace.xz);
            BezierPath bezierPath = new BezierPath(pointsPos, false, PathSpace.xz);
            road.GetComponent<PathCreator>().bezierPath = bezierPath;
            //newPoint.y = terrain.SampleHeight(newPoint);
            
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
            
            GameObject road2 = new GameObject("RoadContainer");
            road2.AddComponent<PathCreator>();
            road2.AddComponent<RoadMeshCreator>();
            road2.AddComponent<ObjectPlacer>();
            road2.GetComponent<RoadMeshCreator>().roadMaterial = Resources.Load<Material>("Road");
            road2.GetComponent<RoadMeshCreator>().undersideMaterial = Resources.Load<Material>("Road");
            road2.GetComponent<RoadMeshCreator>().textureTiling = 100.0f;
            terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
                
            Vector3 newPoint2 = MathUtility.TransformPoint(pointsPos, transform, PathSpace.xz);
            BezierPath bezierPath2 = new BezierPath(newPoint2);
            road2.GetComponent<PathCreator>().bezierPath = bezierPath2;
            newPoint2.y = terrain.SampleHeight(newPoint2);
            
            bezierPath2.SetPoint(0, new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10)));
            bezierPath2.SetPoint(1,
                new Vector3(Random.Range(1, terrain.terrainData.size.x / 3), 0,
                    Random.Range(1, terrain.terrainData.size.z / 3)));
            bezierPath2.SetPoint(2,
                new Vector3(Random.Range(2, terrain.terrainData.size.x / 2), 0,
                    Random.Range(2, terrain.terrainData.size.z / 2)));
            bezierPath2.SetPoint(3,
                new Vector3(Random.Range(3, terrain.terrainData.size.x), 0,
                    Random.Range(3, terrain.terrainData.size.z)));
            
            bezierPath2.ControlPointMode = BezierPath.ControlMode.Automatic;
            
            if (bezierPath.IsIntersecting(bezierPath))
            {
                //bezierPath.SplitSegment(bezierPath.PathBounds.center, 0, 1f);

                /*Vector3[] newPoints;

                bezierPath.CombineTwoBeziers(bezierPath.GetPointsInSegment(0), bezierPath2.GetPointsInSegment(0), out newPoints);#1#
                
                bezierPath.AddPath(bezierPath2);
                
                print("Anchor points : " + bezierPath.NumAnchorPoints);
                print("Points : " + bezierPath.NumPoints);
                print("Segments : " + bezierPath.NumSegments);
                
                print("Anchor points : " + bezierPath2.NumAnchorPoints);
                print("Points : " + bezierPath2.NumPoints);
                print("Segments : " + bezierPath2.NumSegments);
            }*/
            
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
                
                //Vector3 newPoint = MathUtility.TransformPoint(pointsPos, transform, PathSpace.xz);
                BezierPath bezierPath = new BezierPath(pointsPos, false, PathSpace.xz);
                road.GetComponent<PathCreator>().bezierPath = bezierPath;
                //newPoint.y = terrain.SampleHeight(newPoint);
                
                /*var points = new Vector3[4];
                for (var j = 0; j < points.Length; j++)
                {
                    points[j] = bezierPath.GetPoint(j);
                    points[j] = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));
                    points[j].y = terrain.SampleHeight(points[j]);
                    bezierPath.AddSegmentToEnd(points[j]);
                    pointsPos = points[j];
                }*/
                
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
                    //bezierPath.SplitSegment(bezierPath.PathBounds.center, 0, 1f);

                    //Vector3[] newPoints2;

                    //bezierPath.CombineTwoBeziers(bezierPath.GetPointsInSegment(0), bezierPath.GetPointsInSegment(0), out newPoints2);
                    
                    //bezierPath.AddPath(bezierPath);
                }
                
                print("Anchor points : " + bezierPath.NumAnchorPoints);
                print("Points : " + bezierPath.NumPoints);
                print("Segments : " + bezierPath.NumSegments);
            }
        }
    }
}
