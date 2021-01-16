using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;

public class Tree : NetworkedBehaviour
{
    private Animator animator;
    NetworkedVarInt hp;
    // Start is called before the first frame update
    void Start()
    {
        hp = new NetworkedVarInt(Random.Range(4,7));
        animator = GetComponent<Animator>();
    }

    public void HitTree() {
        InvokeServerRpc(TakeDamage);
    }

    [ServerRPC(RequireOwnership = false)]
    public void TakeDamage() {
        hp.Value -= 1;
        animator.SetTrigger("hit");
        if (hp.Value == 0) {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
