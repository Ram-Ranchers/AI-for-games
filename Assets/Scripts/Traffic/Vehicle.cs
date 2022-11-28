using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] public float vehicleLength;
    [SerializeField] public float vehicleSpeed;
    private List<Junction> path;

    public float distanceAlongRoad = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void findPath(List<Junction> junctions,Junction targetJunction)
    {
        List<Junction> openPath = new List<Junction>();
        List<Junction> closedPath;

        float gCost = 0.0f, hCost = 0.0f, fCost = 0.0f;
        Junction nextJunction = junctions[0];//change to end junction of currrent spline when implemented
        float nearDist = Vector3.Distance(nextJunction.transform.position, this.transform.position);
        for (int i = 0; i < junctions.Count; i++)
        {
            if (Vector3.Distance(junctions[i].transform.position,this.transform.position)<nearDist)
            {
                nearDist = Vector3.Distance(junctions[i].transform.position, this.transform.position);
                nextJunction = junctions[i];
            }
        }

        openPath.Add(nextJunction);
        for (int i = 0; i < junctions.Count*junctions.Count; i++)
        {

        }
    }
}
