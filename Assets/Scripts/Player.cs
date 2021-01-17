using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Player : NetworkedBehaviour
{

    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform attackPoint;
    private AudioManager audioManager;
    private float stunTimer;
    private bool stunned;

    public LayerMask treeLayer;
    public bool hacking = false;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attackPoint = transform.Find("AttackPoint");
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void CheckInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");


        if (Input.GetMouseButton(0))
        {
            animator.SetBool("hacking", stunned ? false : true);
            hacking = stunned ? false : true;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - transform.position).normalized;
            transform.rotation = (direction.x < 0) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        }
        else {
            animator.SetBool("hacking", false);
        }
    }

    public void Hack()
    {
        if (IsLocalPlayer)
        {// logic for creating overlap circle
        Collider2D[] hitEntities = Physics2D.OverlapCircleAll(attackPoint.position, 0.5f, treeLayer);
        foreach (Collider2D entity in hitEntities)
        {
            audioManager.Play("TreeHit");
            entity.GetComponent<Tree>().HitTree(OwnerClientId);
        }}
    }
    // Update is called once per frame
    void Update()
    {

        if (IsLocalPlayer)
        {
        
            CheckInput();
            Move();
            if (stunTimer > 0) {
                rb.velocity = Vector2.zero;
                stunTimer -= Time.deltaTime;
            } else {
                animator.SetBool("stunned", false);
                stunned = false;
            }
        }
    }

    void Move()
    {
        rb.velocity = hacking ? Vector2.zero : movement * 2f;

        if (rb.velocity != Vector2.zero)
        {
            if (rb.velocity.x < -0.1f || rb.velocity.x > 0.1f)
                transform.rotation = (movement.x < 0) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            animator.SetBool("moving", hacking || stunned ? false : true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    public void Stunned(){
        animator.SetBool("stunned", true);
        stunTimer = .5f;
        stunned = true;
    }
}
