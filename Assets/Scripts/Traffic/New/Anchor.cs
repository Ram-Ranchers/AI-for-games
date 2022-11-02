using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    [SerializeField] public GameObject handle1, handle2;
    [SerializeField] public GameObject pos;
    // Start is called before the first frame update
    public Anchor(GameObject handle1, GameObject handle2, GameObject pos)
    {
        this.handle1 = handle1;
        this.handle2 = handle2;
        this.pos = pos;
    }

    void Start()
    {

    }

    private void Update()
    {
    }
}

  
