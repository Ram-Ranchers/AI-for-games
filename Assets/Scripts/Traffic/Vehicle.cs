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
    private float stoppingDistance = 10;

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
        float maxSpeed = 100f;
        float acceleration = 10f;
        float deceleration = 0;
        bool decelerating = false;


        if(vehicleSpeed < maxSpeed && !decelerating)
        {
            vehicleSpeed += acceleration * Time.deltaTime;
        }
        else
        {
            vehicleSpeed = maxSpeed;
        }

        //broken
        if(Vector3.Distance(transform.position, PointA.position) <= stoppingDistance && vehicleSpeed > 0)
        {
            decelerating = true;
            deceleration += acceleration;
        }
        else
        {
            decelerating = false;
            deceleration = 0;
        }

        transform.Translate(transform.forward * vehicleSpeed * Time.deltaTime);
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
