using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{


    public int speed;
    public int direction;
    private Rigidbody2D rb;
    private float hitDist = 0.7f;
    public Animator animator;
    public bool facingLeft;

    // Start is called before the first frame update
    void Start()
    {
        facingLeft = true;
        rb = GetComponent<Rigidbody2D>();
        speed = 2;
        direction = -1;
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        CheckY();
    }

    private void CheckY()
    {
        // this function will check the y value of the player, as well as the y velocity
        // this is actual dogshit garbage and you should feel bad for using this
        //if ((Vector3)rb.velocity == Vector3.zero) isGrounded = true; 
        //else isGrounded = false;
        int yDead = -7;
        if (transform.position.y <= yDead)
        {
            Destroy(gameObject);
        }
    }

    void Move()
    {
        // check the distance between enemy and the object its colliding with
        // if the distance is 0.7, do something
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, 
            new Vector2(
                direction, 0
            )
        );

        rb.velocity = new Vector2(
            direction, 0
        ) * speed;

        if (hit.distance < hitDist)
        {
            Flip();
            if(hit.collider.tag == "Player")
            {
                KillPlayer();
                //Destroy(hit.collider.gameObject);
            }
        } 
    }

    private void KillPlayer()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // so using tilemaps, we can make new tilemaps and assign different tags to them, for ex: water and ground.
        //Debug.Log("enemy has collided with " + collision.collider.name + " with tag: " + collision.gameObject.tag);
        //if (collision.gameObject.tag == "groundable");
    }


    void Flip()
    {
        facingLeft = !facingLeft;
        direction *= -1;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    
}
