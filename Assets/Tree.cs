using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;

public class Tree : NetworkedBehaviour
{

    public GameObject particles;
    private Animator animator;
    private AudioManager audioManager;
    UIManager manager;
    NetworkedVarInt hp;
    // Start is called before the first frame update
    void Start()
    {
        hp = new NetworkedVarInt(Random.Range(7,10));
        animator = GetComponent<Animator>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        manager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    public void HitTree(ulong clientId) {
        InvokeServerRpc(TakeDamage, clientId);
    }

    [ServerRPC(RequireOwnership = false)]
    public void TakeDamage(ulong clientId) {
        hp.Value -= 1;
        manager.AddWood(clientId, 1);
        animator.SetTrigger("hit");
        if (hp.Value == 0) {
            manager.AddWood(clientId, 5);
            manager.treeDestroyed();
            GameObject particleInstance = Instantiate(particles, transform.position, Quaternion.identity);
            audioManager.Play("TreeBreak");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

}
