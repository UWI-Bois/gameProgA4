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
        //rb.mass = 300;
        //rb.gravityScale = 5;
        //print("loaded: " + attributes.name);

        if (attributes.name == "slime") attributes.hp += (int)gameObject.transform.localScale.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (attributes.isDead) return; // check this first
        attributes.jumpForce = 20;
        CheckStates();
        CheckY();
        if(!attributes.isFalling && attributes.isGrounded) Move();
        //if (attributes.isStuck) Jump(); // check this last

        if (attributes.name == "dio" && attributes.isEnraged && attributes.hp != attributes.enragedHP)
            Physics2D.IgnoreLayerCollision(attributes.layerNum, 13, false);

        Fall();
        if (rb.gravityScale > 1 && !attributes.isFalling) rb.gravityScale = 1;
    }

    private void CheckStates()
    {
        CheckVertical();
        CheckHorizontal();
        CheckDamaged();
        CheckStuck();
    }

    private void CheckDamaged()
    {
        if (attributes.hp != attributes.maxHp) attributes.isDamaged = true;
        else attributes.isDamaged = false;
        if (attributes.canRage && attributes.hp <= attributes.enragedHP) attributes.Enrage();
    }

    private void CheckHorizontal()
    {
        float vel = Mathf.Abs(rb.velocity.x);
        if (vel > 0.01) // going right
        {
            attributes.facingLeft = false;
        }
        if (rb.velocity.x < 0.01) // going left
        {
            attributes.facingLeft = true;
        }
    }

    private void CheckVertical()
    {
        //print("checking vertical, rb.y = " + rb.velocity.y);
        if (rb.velocity.y > 0.01) // jumping
        {
            attributes.isGrounded = true;
            attributes.isFalling = false;
        }
        else if (rb.velocity.y < 0.01)
        {
            attributes.isGrounded = false;
            attributes.isFalling = true;
        }
        else if (rb.velocity.y == 0)
        {
            attributes.isGrounded = true;
            attributes.isFalling = false;
        }
        animator.SetBool("isGrounded", attributes.isGrounded);
        if (attributes.isFalling) Fall();
    }

    private void Update()
    {
        initPos = gameObject.transform.position;
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

    void Jump()
    {
        print(attributes.name + " is jumping with force = " + attributes.jumpForce);
        rb.gravityScale = 1;
        rb.mass = 1;
        rb.AddForce(Vector2.up * attributes.jumpForce);
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
        //if(attributes.isStuck)
            attributes.speed = attributes.maxSpeed;

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
        if (rb.velocity == Vector2.zero && !attributes.isFalling)
        {
            //Jump();
            attributes.isStuck = true;
            //attributes.speed = 0;
        }
        else if (Math.Abs(rb.velocity.x) < 0.01 && !attributes.isFalling)
        {
            //Jump();
            attributes.isStuck = true;
            //attributes.speed = 0;
        }
        else attributes.isStuck = false;
        if (attributes.isStuck) UnStick();
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
        //if (collision.gameObject.tag == "groundable") GroundEnemy();
        if (collision.gameObject.tag == "hangable" || collision.gameObject.tag.Contains("breakable")) Flip();
        if (collision.gameObject.tag.Contains("Enemy")) WalkThrough(collision);
        if (collision.gameObject.tag == "Player") StartCoroutine(Attack());
    }

    void Fall()
    {
        if (!attributes.isFalling || attributes.isGrounded) return;
        print(attributes.name + " is falling with force = " + attributes.jumpForce);
        rb.gravityScale = 1;
        rb.mass = 1;
        rb.AddForce(Vector2.down * attributes.jumpForce);
        CheckVertical();
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
        if (!attributes.wasEaten)
            Player.instance.IncreaseEXP(attributes.expVal);
        Destroy(gameObject);
    }
    public IEnumerator UnStick()
    {
        yield return new WaitForSeconds(2);
        Jump();
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
