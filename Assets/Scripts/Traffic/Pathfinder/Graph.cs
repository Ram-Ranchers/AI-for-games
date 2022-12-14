using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public Dictionary<GraphNode, GraphNode[]> edges = new Dictionary<GraphNode, GraphNode[]>();

    public GraphNode[] Neighbors(GraphNode id)
    {
        return edges[id];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
