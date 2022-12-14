using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnchangedSplines;
//struct Road
//{
//    public int lanesLeft,lanesRight;
//    public Vector3 end1,end2;
//    public List<Vehicle> vehicles;
//}

struct QuadGen
{
    int road1, road2;
    Vector3 start,intersect,end;
}

public class Junction : MonoBehaviour
{
    // attributes :: road connections anchors ; list of paths ;

    PathCreator pathCreator;

    private List<GameObject> connectedRoads;
    private List<Vector3> roadConnections;
    private List<QuadGen> traversalPathsQuads;

    private List<Vehicle> vehiclesInJunction;
    private List<bool> activeRoads;

    [SerializeField] private float cycleTime = 10.0f;
    private float cycleLast;

    private int lastActiveRoad;

    private void Start()
    {
        cycleLast = Time.time;
    }

    public void GenerateJunction(GameObject roadContainer1,GameObject roadContainer2)
    {

    }

    public void addRoad()
    {

    }

    public bool exitClear()
    {
        //get vehicle planned route then check the exit road for space/ also check if there will be space if the car starts moving now

        return true;
    }

    private void Update()
    {
        //on some trigger call pathgeneration, need to make a tool or soemthing?

        if(cycleLast+cycleTime>Time.time)
        {
            LightCycle();
            //update road stoppage for connecting roads

        }
        
    }

    //cycle traffic control
    void LightCycle()
    {
        //TODO Pedestrian stuff?
        for (int i = 0; i < activeRoads.Count; i++)
        {
            activeRoads[i] = !activeRoads[i];
        }
    }

    //generate pathways
    void PathGeneration()
    {
        //for(int i = 0; i < connectedRoads.Count; i++)
        //{
        //    for (int j = 0; j < connectedRoads[i].lanesLeft; j++)
        //    {
        //        for (int k = 0; k < connectedRoads.Count; k++)
        //        {
        //            if (i == k)
        //                continue;
        //            for (int l = 0; l < connectedRoads[k].lanesRight; l++)
        //            {
        //                //gen spline path using vehicle heading 
        //            }
        //        }
        //    }
        //}
        
        for (int i = 0; i < connectedRoads.Count; i++)
        {
            for (int j = i+1; j < connectedRoads.Count; j++)
            {

            }
        }
    }
}
