using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spline;

struct Road
{
    public int lanesLeft,lanesRight;
    public Vector3 end1,end2;
    public List<Vehicle> vehicles;
}

struct QuadGen
{
    int road1, road2;
    Vector3 start,intersect,end;
}

public class Junction : MonoBehaviour
{
    // attributes :: road connections anchors ; list of paths ;

    private List<Road> connectedRoads;
    private List<QuadGen> traversalPaths;
    private List<Vehicle> vehiclesInJunction;
    private List<bool> activeRoads;

    [SerializeField] private float cycleTime = 10.0f;
    private float cycleLast;

    private int lastActiveRoad;

    private void Start()
    {
        cycleLast = Time.time;
    }

    private void Update()
    {
        //on some trigger call pathgeneration, need to make a tool or soemthing?

        if(cycleLast+cycleTime>Time.time)
        {
            lightCycle();
        }

    }

    //cycle traffic control
    void lightCycle()
    {
        //TODO Pedestrian stuff?
        for (int i = 0; i < activeRoads.Count; i++)
        {
            activeRoads[i] = !activeRoads[i];
        }
    }


    //generate pathways
    void pathGeneration()
    {
        for(int i = 0; i < connectedRoads.Count; i++)
        {
            for (int j = 0; j < connectedRoads[i].lanesLeft; j++)
            {
                for (int k = 0; k < connectedRoads.Count; k++)
                {
                    if (i == k)
                        continue;
                    for (int l = 0; l < connectedRoads[k].lanesRight; l++)
                    {
                        //gen spline path using vehicle heading 
                    }
                }
            }
        }
    }
}
