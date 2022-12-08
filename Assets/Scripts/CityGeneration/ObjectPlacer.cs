using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    void Start()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point + 0.0f * hit.normal;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else
        {
            ray = new Ray(transform.position, transform.up);
            if (Physics.Raycast(ray, out hit))
            {
                transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
        }
    }
}
