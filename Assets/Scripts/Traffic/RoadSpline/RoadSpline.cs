using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSpline : MonoBehaviour
{
    [SerializeField] GameObject junction1, junction2;
    [SerializeField] GameObject handle1, handle2;
    [SerializeField] List<Anchor> anchors;
    [SerializeField] GameObject pos;
    private float t =0.01f;
    // Start is called before the first frame update
    

    void Start()
    {
        Anchor temp = new Anchor(handle2, handle1, junction2);
        anchors.Add(temp);
        temp = new Anchor(handle1, handle1, junction1);
        anchors.Add(temp);
        Debug.Log("start");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(anchors.Count);
        t=(t+Time.deltaTime)%(anchors.Count-1);
        if (anchors[0] != null)
            Debug.Log("here");
        if (anchors[1] != null)
            Debug.Log("here2");
        int index = Mathf.CeilToInt(t) % (anchors.Count - 1);
        pos.transform.position = cubicLerp(anchors[index].pos.transform.position, anchors[index].handle2.transform.position, anchors[(index + 1) % (anchors.Count-1)].handle1.transform.position, anchors[(index + 1) % (anchors.Count-1)].pos.transform.position, t);
    }

    private Vector3 quadLerp(Vector3 a, Vector3 b, Vector3 c, float t)//a and c are positions b is the handle
    {
        return Vector3.Lerp(Vector3.Lerp(a, b, t), Vector3.Lerp(b, c, t), t);
    }
    private Vector3 cubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)//
    {
        return Vector3.Lerp(quadLerp(a, b,c, t), quadLerp(b, c, d, t), t);
    }
}
