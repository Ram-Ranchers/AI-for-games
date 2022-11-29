using System.Collections.Generic;
using UnityEngine;
using BetterSpline;

public class RoadGen : MonoBehaviour
{
    public Terrain terrain;
    public List<Transform> waypoints;

    private GameObject obj;
    private int numOfObj = 20;

    private void Start()
    {
        obj = new GameObject("WayPoint");

        for (int i = 0; i < 5; i++)
        {
            Instantiate(obj, new Vector3(Random.Range(0, 100), 0, Random.Range(0, 100)),
                Quaternion.identity);
            waypoints.Add(obj.transform);
        }

        if (waypoints.Count > 0)
        {
            BezierPath bezierPath = new BezierPath(waypoints);
            GetComponent<PathCreator>().bezierPath = bezierPath;
        }
    }
}
