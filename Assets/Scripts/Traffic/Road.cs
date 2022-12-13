using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BetterSpline;

public class Road : MonoBehaviour
{
    public int lanesLeft, lanesRight;
    public Vector3 end1, end2;
    public List<Vehicle> leftVehicles, rightVehicles;
    private float maxSpeed = 30;
    private int size;
    private PathCreator pathCreator;
    private EndOfPathInstruction end;

    // Start is called before the first frame update
    void Start()
    {
        pathCreator = gameObject.GetComponent<PathCreator>();
        end = EndOfPathInstruction.Stop;
        end1 = pathCreator.path.GetPointAtDistance(0, end);
        end2 = pathCreator.path.GetPointFromEnd(0, end);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }
}
