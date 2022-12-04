using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    //Driving shit
    [SerializeField] public float vehicleLength;
    [SerializeField] public float vehicleSpeed;
    private List<Junction> path;

    public float distanceAlongRoad = 0;

    public Transform PointA;
    private float slowingDistance = 50f;
    private float maxSpeed = 100f;

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

        else
        {
            vehicleSpeed += acceleration * Time.deltaTime;
        }

        transform.Translate(transform.forward * Time.deltaTime * vehicleSpeed * speedPercent);
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
