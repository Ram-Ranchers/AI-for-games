using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class Curve
{
    Vector3 anchor1, anchor2, handle1, handle2;

    Vector3 QuadLerp(Vector3 anchor1, Vector3 anchor2, Vector3 handle, float t)
    {
        return Vector3.Lerp(Vector3.Lerp(anchor1, handle, t), Vector3.Lerp(anchor2, handle, t),t);
    }



    Vector3 GetPointAt(Curve bezierCurve, float t)
    {

    }
}
