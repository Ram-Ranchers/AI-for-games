using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : ScriptableObject
{
    [SerializeField] public GameObject handle1, handle2;
    [SerializeField] public GameObject pos;
    // Start is called before the first frame update
    public Anchor(GameObject handle1, GameObject handle2, GameObject pos)
    {
            this.handle1.transform.position = handle1.transform.position;
            this.handle2.transform.position = handle2.transform.position;
        this.pos.transform.position = pos.transform.position;
    }
}

  
