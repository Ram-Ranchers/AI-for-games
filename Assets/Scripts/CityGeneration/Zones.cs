using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CityGeneration;

public class Zones : MonoBehaviour
{
    public int buildingType;

    public Building building;

    public Buildings buildings;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, 100);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == buildings.terrain)
        {
            building.buildingType = buildingType;
        }
    }

    private void OnClickMove()
    {

    }
}
