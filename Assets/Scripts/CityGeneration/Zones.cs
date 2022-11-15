using UnityEngine;

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
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 0, 0, 0.5f);
            Gizmos.DrawCube(transform.position, size);
        }
    }
}
