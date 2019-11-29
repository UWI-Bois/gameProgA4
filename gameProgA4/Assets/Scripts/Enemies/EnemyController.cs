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
    public BoxCollider2D boxCollider;
    public int massNum;

    // Start is called before the first frame update
    void Start()
    {
        massNum = 5;
        
        //boxCollider = GetComponent<BoxCollider2D>();
        //print(boxCollider.ToString());
        if (gameObject.tag.Contains("Slime")) attributes = GetComponent<SlimeAttr>();
        if (gameObject.tag.Contains("Skeleton")) attributes = GetComponent<SkeletonAttr>();
        if (gameObject.tag.Contains("Dio")) attributes = GetComponent<DioAttr>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
        rb.mass = 300;
        //rb.gravityScale = 5;
        //print("loaded: " + attributes.name);

        if (attributes.name == "slime") attributes.hp += (int)gameObject.transform.localScale.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (attributes.isDead) return; // check this first
        
        attributes.jumpForce = 20;
        if (rb.mass != massNum) rb.mass = massNum;
        if (attributes.canRage && attributes.hp <= attributes.enragedHP) attributes.Enrage();
        

        if (Mathf.Abs(rb.velocity.y) < 0.3) GroundEnemy();
        else attributes.isGrounded = false;

        CheckY();
        
        
        if(attributes.isGrounded)Move();
        //CheckStuck(); // ENABLE THIS
        //if (attributes.isStuck) Jump(); // check this last
        //checkRaycast();
        if (rb.velocity.magnitude < 0.01) Jump();

        if (attributes.name == "dio" && attributes.isEnraged && attributes.hp != attributes.enragedHP)
            Physics2D.IgnoreLayerCollision(attributes.layerNum, 13, false);

        Fall();
    }

    private void Update()
    {
        initPos = gameObject.transform.position;
        //if (attributes.canRage && attributes.hp <= attributes.enragedHP) attributes.Enrage();
        //SleepEnemy(); // ENABLE THIS
        //if (attributes.isStuck) Jump();
    }

    public void GetEaten(int amt)
    {
        attributes.hp -= amt;
        attributes.isDamaged = true;
        animator.SetBool("isDamaged", attributes.isDamaged);
        if (attributes.hp > 0) attributes.PlayHit();
        if (attributes.hp <= 0){
            attributes.wasEaten = true;
            Die();
        }
        attributes.Enrage();
    }

    void UnStick()
    {
        Vector3 newPos = new Vector3(
            gameObject.transform.position.x,
            gameObject.transform.position.y + 1,
            0
            );
        gameObject.transform.position = newPos;
        attributes.isStuck = false;
    }

    void GroundEnemy()
    {
        attributes.isGrounded = true;
        animator.SetBool("isGrounded", true);
    }

    void Jump()
    {
        print(attributes.name + " is jumping with force = " + attributes.jumpForce);
        rb.gravityScale = 1;
        rb.mass = 1;
        rb.AddForce(Vector2.up * attributes.jumpForce);
        attributes.isGrounded = false;
        attributes.isStuck = false;
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
        attributes.speed = attributes.maxSpeed;
        //if (attributes.isFalling) return;
        rb.velocity = new Vector2(
            attributes.direction, 0
        ) * attributes.speed;
        initPos = rb.position;
        animator.SetFloat("velX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("velY", rb.velocity.y);
        //if (attributes.isStuck) Jump();
    }

    void CheckStuck()
    {
        if (rb.velocity == Vector2.zero)
        {
            //Jump();
            attributes.isStuck = true;
        }
        else if (Math.Abs(rb.velocity.x) < 0.01)
        {
            //Jump();
            attributes.isStuck = true;
        }
        else if (initPos == rb.position)
        {
            print("same pos stuck");
            attributes.isStuck = true;
        }

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
        if (collision.gameObject.tag == "hangable" || collision.gameObject.tag.Contains("breakable")) Flip();
        if (collision.gameObject.tag.Contains("Enemy")) WalkThrough(collision);
        if (collision.gameObject.tag == "Player") StartCoroutine(Attack());
    }

    void Fall()
    {
        if(Mathf.Abs(rb.velocity.y) > 0.01)
        {
            attributes.speed = 0;
            attributes.isFalling = true;
            rb.gravityScale++;
        }
        else
        {
            attributes.isFalling = false;
            attributes.speed = attributes.maxSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag.Contains("Enemy")) Physics.IgnoreCollision(boxCollider, collision.collider);
    }

    void WalkThrough(Collision2D col)
    {
        //Physics.IgnoreCollision(gameObject.collider, )
        EnemyController eCol = col.gameObject.GetComponent<EnemyController>();
        //print("Attempting to walkthrough: collided with: " + eCol.attributes.toString());
        if (attributes.name == "dio" && attributes.isEnraged)
        {
            DioController dioC = gameObject.GetComponent<DioController>();
            dioC.EatFriend(col.gameObject);
            //Physics2D.IgnoreLayerCollision(attributes.layerNum, eCol.attributes.layerNum, false);
            return;
        }
        else Physics2D.IgnoreLayerCollision(attributes.layerNum, eCol.attributes.layerNum, true);
        
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
        //if (attributes.canRage && attributes.hp <= attributes.enragedHP) attributes.Enrage();
        attributes.isDamaged = true;
        animator.SetBool("isDamaged", attributes.isDamaged);
        if (attributes.hp > 0) attributes.PlayHit();
        if (attributes.hp <= 0) Die();
    }

    public IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(2);
        attributes.PlayDead();
        if(!attributes.wasEaten)
            Player.instance.IncreaseEXP(attributes.expVal);
        Destroy(gameObject);
    }

    void Die()
    {
        if(!attributes.isDead && !attributes.wasEaten) AudioManager.instance.PlayOra();
        if(!attributes.isDead && attributes.wasEaten) attributes.PlayEat();
        attributes.isDead = true;
        attributes.isDamaged = false;
        animator.SetBool("isDead", true);
        animator.SetBool("isDamaged", false);
        if (attributes.name == "dio") GameManager.instance.win = true;
        StartCoroutine(DestroyEnemy());
    }
}
