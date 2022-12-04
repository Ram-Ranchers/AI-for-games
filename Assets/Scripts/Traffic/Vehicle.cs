using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BetterSpline;

public class Vehicle : MonoBehaviour
{
    //Driving shit
    [SerializeField] public float vehicleLength;
    [SerializeField] public float vehicleSpeed;
    private List<Junction> path;

    public Transform PointA;
    private float slowingDistance = 50f;
    private float maxSpeed = 30f;

    public PathCreator pathCreator;
    public EndOfPathInstruction end;
    public float distanceAlongRoad = 0;

    private bool isOpposite = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {        
        float acceleration = 10f;
        float speedPercent = 1f;

        Vector3 targetDir = PointA.position - transform.position;
        float dot = Vector3.Dot(targetDir, transform.forward);

        if(dot < 5f && DistanceFromPoint(PointA.position) <= slowingDistance)
        {
            speedPercent = Mathf.Clamp01(DistanceFromPoint(PointA.position) / slowingDistance);

            if (speedPercent < 0.1f)
            {
                vehicleSpeed = 0f;
            }
        }

        else if(vehicleSpeed < maxSpeed)
        {
            vehicleSpeed += acceleration * Time.deltaTime;
        }

        distanceAlongRoad += vehicleSpeed * speedPercent * Time.deltaTime;
        transform.position = new Vector3(pathCreator.path.GetPointAtDistance(distanceAlongRoad, end).x, pathCreator.path.GetPointAtDistance(distanceAlongRoad, end).y + 1, pathCreator.path.GetPointAtDistance(distanceAlongRoad, end).z);

        //fix rotation to follow spline properly
        transform.rotation = new Quaternion(transform.rotation.x, pathCreator.path.GetRotationAtDistance(distanceAlongRoad, end).y, transform.rotation.z, 1);

        //get it to work by flipping direction

        if(isOpposite)
        {
            distanceAlongRoad += vehicleSpeed * speedPercent * Time.deltaTime;
            transform.position = new Vector3(pathCreator.path.GetPointAtDistance(distanceAlongRoad, end).x, pathCreator.path.GetPointAtDistance(distanceAlongRoad, end).y + 1, pathCreator.path.GetPointAtDistance(distanceAlongRoad, end).z);
            transform.rotation = new Quaternion(transform.rotation.x, pathCreator.path.GetRotationAtDistance(distanceAlongRoad, end).y, transform.rotation.z, 1);
        }
    }

    private float DistanceFromPoint(Vector3 point)
    {
        return Vector3.Distance(transform.position, point);
    }    

    void findPath(List<Junction> junctions, Junction targetJunction)
    {
        List<Junction> openPath = new List<Junction>();
        List<Junction> closedPath;

        float gCost = 0.0f, hCost = 0.0f, fCost = 0.0f;
        Junction nextJunction = junctions[0];//change to end junction of currrent spline when implemented
        float nearDist = Vector3.Distance(nextJunction.transform.position, this.transform.position);
        for (int i = 0; i < junctions.Count; i++)
        {
            if (Vector3.Distance(junctions[i].transform.position, this.transform.position) < nearDist)
            {
                nearDist = Vector3.Distance(junctions[i].transform.position, this.transform.position);
                nextJunction = junctions[i];
            }
        }

        openPath.Add(nextJunction);
        for (int i = 0; i < junctions.Count * junctions.Count; i++)
        {

        }
    }
}
