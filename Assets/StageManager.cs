using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class StageManager : NetworkedBehaviour
{

    public GameObject tree;
    public GameObject rocks;
    public GameObject beaver;
    public bool initiate;
    public LayerMask treeLayer;
    UIManager manager;


    private float beaverSpawnTimer = 8f;
    private GameObject beaverInstance;
    private Vector2 beaverSpawnPos;
    private bool beaverSpawned = false;
    private int beaverCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer && manager.roundActive == true)
        {
            beaverSpawnTimer += Time.deltaTime;
            if (beaverSpawnTimer > 10f && beaverCount<5)
            {
                beaverSpawnTimer = 0f;
                beaverSpawnPos.x = Random.Range(-5f, 5f);
                beaverSpawnPos.y = Random.Range(-2f, 2f);

                Collider2D[] treeCheck = Physics2D.OverlapCircleAll(new Vector3(beaverSpawnPos.x, beaverSpawnPos.y, 0), .75f, treeLayer);
                while (treeCheck.Length > 1)
                {
                    beaverSpawnPos.x = Random.Range(-5f, 5f);
                    beaverSpawnPos.y = Random.Range(-2f, 2f);

                    treeCheck = Physics2D.OverlapCircleAll(new Vector3(beaverSpawnPos.x, beaverSpawnPos.y, 0), .75f, treeLayer);
                }
                beaverInstance = Instantiate(beaver, new Vector3(beaverSpawnPos.x, beaverSpawnPos.y + 20f, 0), Quaternion.identity);
                beaverInstance.GetComponent<NetworkedObject>().Spawn();

                beaverInstance.GetComponent<CapsuleCollider2D>().enabled = false;
                beaverInstance.GetComponent<Rigidbody2D>().gravityScale = .5f;
                beaverInstance.GetComponent<BeaverAi>().enabled = false;
                beaverSpawned = true;
                beaverCount++;
            }

            if (beaverSpawned == true)
            {
                if (beaverInstance.transform.position.y <= beaverSpawnPos.y)
                {
                    beaverInstance.GetComponent<CapsuleCollider2D>().enabled = true;
                    beaverInstance.GetComponent<Rigidbody2D>().gravityScale = 0f;
                    beaverInstance.GetComponent<BeaverAi>().enabled = true;

                    beaverSpawned = false;
                }


            }
        }

    }

    public void GenerateStage()
    {
        // maxmimum of 32 trees
        float x;
        float y;
        bool spawnTree = true;
        for (int i = 0; i < 32; i++)
        {
            x = Random.Range(-8f, 8f);
            y = Random.Range(-3.5f, 3.5f);

            Collider2D[] treeCheck = Physics2D.OverlapCircleAll(new Vector3(x, y, 0), .75f, treeLayer);
            foreach (Collider2D entity in treeCheck)
            {
                if (entity.gameObject.layer == LayerMask.NameToLayer("Tree"))
                {
                    spawnTree = false;
                    break;
                }
            }

            if (spawnTree)
            {
                manager.treeCounter++;

                GameObject treeInstance = Instantiate(tree, new Vector3(x, y, 0), Quaternion.identity);
                treeInstance.GetComponent<NetworkedObject>().Spawn();
            }
            spawnTree = true;

        }
        
        foreach (GameObject beaver in GameObject.FindGameObjectsWithTag("Beaver")) {
            Destroy(beaver);
        }
    }
}
