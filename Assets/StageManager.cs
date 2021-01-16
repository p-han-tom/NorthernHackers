using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class StageManager : MonoBehaviour
{

    public GameObject tree;
    public GameObject rocks;
    public GameObject beaver;
    public bool initiate;
    public LayerMask treeLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (initiate) {
            initiate = false;
            GenerateStage();
            GameObject beaverInstance = Instantiate(beaver, new Vector3(5,0,0), Quaternion.identity);
            beaverInstance.GetComponent<NetworkedObject>().Spawn();
        }
    }

    void GenerateStage() {
        // maxmimum of 32 trees
        float x;
        float y;
        bool spawnTree = true;
        for (int i = 0; i < 24; i ++) {
            x = Random.Range(-8f,8f);
            y = Random.Range(-3.5f,3.5f);

            Collider2D[] treeCheck = Physics2D.OverlapCircleAll(new Vector3(x,y,0), .75f, treeLayer);
            foreach (Collider2D entity in treeCheck) {
                if (entity.gameObject.layer == LayerMask.NameToLayer("Tree")) {
                    spawnTree = false;
                    break;
                }
            }

            if (spawnTree) {
                GameObject treeInstance = Instantiate(tree, new Vector3(x,y,0), Quaternion.identity);
                treeInstance.GetComponent<NetworkedObject>().Spawn();
            }
            spawnTree = true;
            

        }
    }
}
