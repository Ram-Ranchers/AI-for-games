using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CityGeneration
{
    public class Zones : MonoBehaviour
    {
        public int buildingType;

        private Building building;

        private GameObject buildingsGameObject;

        private Buildings buildings;
        
        public Vector3 size;

        private BoxCollider boxCollider;

        private bool buildingsInRange;
        
        private void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.size = size;

            buildingsGameObject = GameObject.FindGameObjectWithTag("Building");
            buildings = buildingsGameObject.GetComponent<Buildings>();
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 0, 0, 0.5f);
            Gizmos.DrawCube(transform.position, size);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Building"))
            {
                buildingsInRange = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Building"))
            {
                buildingsInRange = false;
            }
        }
    }
}
