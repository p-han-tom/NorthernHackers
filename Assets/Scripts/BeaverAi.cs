using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaverAi : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    float normalSpeed = 1f;
    float chargeSpeed = 2f;
    float speed;
    enum beaverState { idle, wander, charge, stunned }
    beaverState state = beaverState.idle;
    float stateTimer = 0;
    Vector2 movement;
    List<GameObject> players;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stateTimer = Random.Range(0, 2f);
        players = new List<GameObject>();
        speed = normalSpeed;
    }
    void Update()
    {
        if (state == beaverState.idle)
        {
            if (stateTimer <= 0)
            {
                stateTimer = Random.Range(1, 2f);
                state = beaverState.wander;
                movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            }
        }
        else if (state == beaverState.wander)
        {
            Move();
            if (stateTimer <= 0)
            {
                stateTimer = Random.Range(1, 2f);
                state = beaverState.idle;
                animator.SetBool("moving", false);
                rb.velocity = Vector2.zero;
            }
        }
        if (state != beaverState.charge && state != beaverState.stunned)
        {
            foreach (GameObject player in players)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.tag == "Player")
                {
                    Debug.Log("jean");
                    state = beaverState.charge;
                    speed = chargeSpeed;
                    movement = (player.transform.position - transform.position).normalized;
                    animator.SetBool("charging", true);
                }
            }

        }
        if (state == beaverState.charge)
        {
            speed+=0.1f;
            Move();
        }
        else if (state == beaverState.stunned)
        {
            if (stateTimer <= 0)
            {
                stateTimer = Random.Range(1, 2f);
                state = beaverState.idle;
                animator.SetBool("moving", false);
                animator.SetBool("stunned", false);
                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Dynamic;
                speed = normalSpeed;
            }
        }
        stateTimer -= Time.deltaTime;
    }
    void Move()
    {
        rb.velocity = movement * speed;
        animator.SetBool("moving", true);
        transform.rotation = (movement.x < 0) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (state == beaverState.wander)
        {
            movement *= -1;
        }
        else if (state == beaverState.charge)
        {
            state = beaverState.stunned;
            animator.SetBool("charging", false);
            animator.SetBool("stunned", true);
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            stateTimer = 2f;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            players.Add(other.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            players.Remove(other.gameObject);
        }
    }
}
