using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaverAi : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    float speed = 1f;
    float chargeSpeed = 3f;
    bool idle = false;
    float idleTime = 0f;
    float wanderingTime = 0f;
    bool enraged = false;
    Vector2 movement;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (enraged == false)
        {
            if (idleTime <= 0) {
                idle = false;
            }
            if (idle == false) {
                WanderDirection();
            }
            if (idle == false && wanderingTime > 0) {
                Move();
            }
            if (wanderingTime <= 0)
            {
                idleTime = Random.Range(1f, 2f);
            }
            if (idleTime > 0)
            {
                animator.SetBool("moving", false);
            }
        }
        idleTime -= Time.deltaTime;
        wanderingTime -= Time.deltaTime;
    }
    void WanderDirection()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        movement = new Vector2(x, y).normalized;
        wanderingTime = Random.Range(1f, 2f);
    }
    void Move()
    {
        rb.velocity = movement * 1f;
        animator.SetBool("moving", true);
        transform.rotation = (movement.x < 0) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }
    void OnCollisionEnter(Collision other)
    {
        if (enraged == false && wanderingTime > 0)
        {
            movement *= -1;
        }
    }
}
