using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnchangedSplines;

public class Vehicle : MonoBehaviour
{
    //Driving shit
    [SerializeField] public float vehicleLength;
    public float vehicleSpeed;
    private float maxSpeed;
    private List<Junction> path;

    //public Transform PointA;
    private float slowingDistance = 50f;

    public PathCreator pathCreator;
    public EndOfPathInstruction end;
    public float distanceAlongRoad = 0;
    private Road currentRoad;

    public bool isOpposite = false;

    // Start is called before the first frame update
    void Start()
    {
        currentRoad = pathCreator.transform.gameObject.GetComponent<Road>();

        maxSpeed = Random.Range(currentRoad.GetMaxSpeed() / 100 * 50, currentRoad.GetMaxSpeed());
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

        //Vector3 targetDir = PointA.position - transform.position;
       // float dot = Vector3.Dot(targetDir, transform.forward);

        Vector3 currentPosition;
        Vector3 lastPosition;

        //if (dot < 5f && DistanceFromPoint(PointA.position) <= slowingDistance)
        //{
        //    speedPercent = Mathf.Clamp01(DistanceFromPoint(PointA.position) / slowingDistance);

        //    if (speedPercent < 0.1f)
        //    {
        //        vehicleSpeed = 0f;
        //    }
        //}

        //else if (vehicleSpeed < currentRoad.GetMaxSpeed())
        //{
        //    vehicleSpeed += acceleration * Time.deltaTime;
        //}

        int layerMask = 1 << 8;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, slowingDistance, layerMask))
        {
            if (DistanceFromPoint(hit.transform.position) <= slowingDistance)
            {
                speedPercent = Mathf.Clamp01(DistanceFromPoint(hit.transform.position) / slowingDistance);

                if (speedPercent < 0.1f)
                {
                    vehicleSpeed = 0f;
                }
            }
        }

        if (vehicleSpeed < maxSpeed)
        {
            vehicleSpeed += acceleration * Time.deltaTime;
        }

        if (!isOpposite)
        {
            lastPosition = transform.position;
            distanceAlongRoad += vehicleSpeed * speedPercent * Time.deltaTime;
            transform.position = new Vector3(pathCreator.path.GetPointAtDistance(distanceAlongRoad, end).x, pathCreator.path.GetPointAtDistance(distanceAlongRoad, end).y + 1.2f, pathCreator.path.GetPointAtDistance(distanceAlongRoad, end).z);
            currentPosition = transform.position;
            Vector3 directionMoving = (currentPosition - lastPosition).normalized;
            transform.rotation = Quaternion.LookRotation(directionMoving);
        }
        //get it to work by flipping direction
        else
        {
            lastPosition = transform.position;
            distanceAlongRoad += vehicleSpeed * speedPercent * Time.deltaTime;
            transform.position = new Vector3(pathCreator.path.GetPointFromEnd(distanceAlongRoad, end).x, pathCreator.path.GetPointFromEnd(distanceAlongRoad, end).y + 1.2f, pathCreator.path.GetPointFromEnd(distanceAlongRoad, end).z);
            currentPosition = transform.position;
            Vector3 directionMoving = (currentPosition - lastPosition).normalized;
            transform.rotation = Quaternion.LookRotation(directionMoving);
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
