using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public int lanesLeft, lanesRight;
    public Vector3 end1, end2;
    public List<Vehicle> leftVehicles, rightVehicles;
    private float maxSpeed = 30;
    private int size;

    // Start is called before the first frame update
    void Start()
    {
        
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
