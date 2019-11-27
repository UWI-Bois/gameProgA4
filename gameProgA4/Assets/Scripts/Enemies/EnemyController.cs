using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    private Rigidbody2D rb;
    public Animator animator;
    public EnemyAttr attributes;
    public Vector2 initPos;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.tag.Contains("Slime")) attributes = GetComponent<SlimeAttr>();
        if (gameObject.tag.Contains("Skeleton")) attributes = GetComponent<SkeletonAttr>();
        if (gameObject.tag.Contains("Dio")) attributes = GetComponent<DioAttr>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
        //print("loaded: " + attributes.name);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckY();
        if (attributes.isDead) return; // if dead, dont check or move
        animator.SetFloat("velX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("velY", rb.velocity.y);
        initPos = rb.position;
        Move();
        //checkRaycast();
        
    }

    private void Update()
    {
        SleepEnemy();
        CheckStuck();
    }

    void SleepEnemy()
    {
        if (rb.velocity == Vector2.zero) rb.Sleep();
    }

    void GroundEnemy()
    {
        attributes.isGrounded = true;
        animator.SetBool("isGrounded", true);
    }

    void Jump()
    {
        rb.gravityScale = 1;
        rb.AddForce(Vector2.up * attributes.jumpForce);
        attributes.isGrounded = false;
        animator.SetBool("isGrounded", attributes.isGrounded);
    }

    private void CheckY()
    {
        int yDead = -7;
        if (transform.position.y <= yDead)
        {
            Destroy(gameObject);
        }
    }

    void Move()
    {
        rb.velocity = new Vector2(
            attributes.direction, 0
        ) * attributes.speed;
        //if (attributes.isStuck) Jump();
    }

    void CheckStuck()
    {
        if (rb.IsSleeping())
        {
            attributes.isStuck = true;
            rb.WakeUp();
            Jump();
        }
        if (rb.velocity == Vector2.zero) Jump();
        //else if (rb.position == initPos) attributes.isStuck = true;
        //else if (rb.velocity.y != 0) attributes.isStuck = true;
        else attributes.isStuck = false;
    }
    public IEnumerator Attack()
    {
        attributes.speed = 0;
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(2);
        attributes.speed = attributes.maxSpeed;
        animator.SetBool("isAttacking", false);
        Flip();
    }

    void checkRaycast()
    {
        // check the distance between enemy and the object its colliding with
        // if the distance is 0.7, do something
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            new Vector2(
                attributes.direction, 0
            )
        );

        if (hit.distance < attributes.hitDist)
        {
            if (hit.collider.tag.Contains("Drop")) return;
            if (hit.collider.tag == "Player")
            {
                //print("Raycast: enemy collided with player");
                StartCoroutine(Attack());
            }
            
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "groundable") GroundEnemy();
        if (collision.gameObject.tag == "hangable") Flip();
        if (collision.gameObject.tag == "Player") StartCoroutine(Attack());
    }

    void Flip()
    {
        attributes.facingLeft = !attributes.facingLeft;
        attributes.direction *= -1;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void TakeDamage()
    {
        attributes.hp -= Player.instance.damage;
        attributes.isDamaged = true;
        animator.SetBool("isDamaged", attributes.isDamaged);
        if (attributes.hp <= 0) Die();
    }

    public IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    void Die()
    {
        attributes.isDead = true;
        attributes.isDamaged = false;
        animator.SetBool("isDead", true);
        animator.SetBool("isDamaged", false);
        Player.instance.IncreaseEXP(attributes.expVal);
        if(attributes.name == "slime" || attributes.name == "skele")
        {
            attributes.PlayDead();
        }
        StartCoroutine(DestroyEnemy());
    }
}
