using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class StageManager : MonoBehaviour
{

    public GameObject tree;
    public GameObject rocks;
    public bool initiate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (initiate) {
            initiate = false;
            GameObject treeInstance = Instantiate(tree, new Vector3(5,0,0), Quaternion.identity);
            treeInstance.GetComponent<NetworkedObject>().Spawn();
        }
    }
}
