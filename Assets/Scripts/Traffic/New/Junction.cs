using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Junction : MonoBehaviour
{
    [SerializeField] int exits = 3;
    [SerializeField] float distFromJunction = 3;
    [SerializeField] List<Junction> connectedJunctions;
    [SerializeField] List<Anchor> junctionAnchors;
    [SerializeField] GameObject anchorPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 anchorDir = new Vector3(-1, 0, 0);
        for (int i = 0; i < exits; i++)
        {
            //GameObject newAnchor = Instantiate(anchorPrefab, this.transform.position + anchorDir * distFromJunction, Quaternion.identity, this.transform);
            GameObject newAnchor = Instantiate(anchorPrefab,this.transform);
            newAnchor.transform.localPosition = anchorDir * distFromJunction;
            newAnchor.GetComponent<Anchor>().pos.transform.localPosition = Vector3.zero;
            newAnchor.GetComponent<Anchor>().handle1.transform.position = anchorDir * (distFromJunction + 5);
            newAnchor.GetComponent<Anchor>().handle2.transform.position = anchorDir * (distFromJunction - 5);
            anchorDir.x = anchorDir.x * Mathf.Cos(2 * Mathf.PI / exits) + anchorDir.z * Mathf.Sin(2 * Mathf.PI / exits);
            anchorDir.z = anchorDir.z * Mathf.Cos(2*Mathf.PI / exits) - anchorDir.x * Mathf.Sin(2 * Mathf.PI / exits);
            Debug.Log(anchorDir.x +" "+ anchorDir.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
