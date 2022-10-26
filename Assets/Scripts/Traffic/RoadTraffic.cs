using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTraffic : MonoBehaviour
{

    [SerializeField] private const int maxVehicles = 10;
    [SerializeField] private float roadLength = 10,carDistance = 0.5f;
    private float lengthOfVehiclesOnRoad = 0;
    private List<Vehicle> vehicles;
    private List<RoadTraffic> roads;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (vehicles[0].distanceAlongRoad + vehicles[0].vehicleSpeed < roadLength)
        {
            vehicles[0].distanceAlongRoad += vehicles[0].vehicleSpeed;
        }
        for (int i = 0; i < vehicles.Count; i++)
        {
            if (vehicles[i].distanceAlongRoad + vehicles[i].vehicleSpeed + carDistance + vehicles[i].vehicleLength < vehicles[i + 1].distanceAlongRoad)
            {
                vehicles[i].distanceAlongRoad += vehicles[i].vehicleSpeed;
            }
        }
    }

    bool addVehicle(Vehicle vehicle)
    {
        if (lengthOfVehiclesOnRoad + vehicle.vehicleLength > roadLength)
        {
            return false;
        }
        else
        {
            lengthOfVehiclesOnRoad += vehicle.vehicleLength;
            vehicles.Add(vehicle);
            return true;
        }
    }
}