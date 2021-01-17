using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;

public class Tree : NetworkedBehaviour
{
    private Animator animator;
    UIManager manager;
    NetworkedVarInt hp;
    // Start is called before the first frame update
    void Start()
    {
        hp = new NetworkedVarInt(Random.Range(7,10));
        animator = GetComponent<Animator>();
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
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

}
