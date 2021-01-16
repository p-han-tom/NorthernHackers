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

    private float beaverSpawnTimer = 8f;
    private GameObject beaverInstance;
    private Vector2 beaverSpawnPos;
    private bool beaverSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        beaverSpawned = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (initiate) {
            initiate = false;
            GenerateStage();
            
        }

        beaverSpawnTimer += Time.deltaTime;
        if (beaverSpawnTimer > 10f) {
            beaverSpawnTimer = 0f;
            beaverSpawnPos.x = Random.Range(-5f,5f);
            beaverSpawnPos.y = Random.Range(-2f,2f);

            Collider2D[] treeCheck = Physics2D.OverlapCircleAll(new Vector3(beaverSpawnPos.x,beaverSpawnPos.y,0), .75f, treeLayer);
            while (treeCheck.Length>1) {
                beaverSpawnPos.x = Random.Range(-5f,5f);
                beaverSpawnPos.y = Random.Range(-2f,2f);

                treeCheck = Physics2D.OverlapCircleAll(new Vector3(beaverSpawnPos.x,beaverSpawnPos.y,0), .75f, treeLayer);
            }
            beaverInstance = Instantiate(beaver, new Vector3(beaverSpawnPos.x,beaverSpawnPos.y+20f,0), Quaternion.identity);
            beaverInstance.GetComponent<NetworkedObject>().Spawn();

            beaverInstance.GetComponent<CapsuleCollider2D>().enabled = false;
            beaverInstance.GetComponent<CircleCollider2D>().enabled = false;

            beaverInstance.GetComponent<Rigidbody2D>().gravityScale = .5f;
            beaverInstance.GetComponent<BeaverAi>().enabled = false;
            beaverSpawned = true;
        }

        if (beaverSpawned == true) {
            if (beaverInstance.transform.position.y <= beaverSpawnPos.y) {
                beaverInstance.GetComponent<CapsuleCollider2D>().enabled = true;
                beaverInstance.GetComponent<CircleCollider2D>().enabled = true;

                beaverInstance.GetComponent<Rigidbody2D>().gravityScale = 0f;
                beaverInstance.GetComponent<BeaverAi>().enabled = true;

                beaverSpawned = false;
            }
            
        
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
