using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Tree : NetworkedBehaviour
{

    private float time = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time>=3f) {
            GetComponent<Animator>().SetTrigger("hit");
            time = 0f;
        }
    }
}
