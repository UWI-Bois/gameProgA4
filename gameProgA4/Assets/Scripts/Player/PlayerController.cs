using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // access ui elements

public class PlayerController  : MonoBehaviour
{
    public int yDead = -10;
    private float moveX;
    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 size;
    public Animator animator;

    public float bottDist;

    public AudioSource coinSound;

    // Start is called before the first frame update
    void Start()
    {
        initController();
        //print(size.ToString() + col.ToString());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move(); // move the player
        CheckY(); // check to see if you fell off the map
        PlayerRaycast(); // used to kill enemies
    }

    void initController()
    {
        DataManagement.dataManagement.LoadData();
        bottDist = 0.5f;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        size = col.bounds.size;
        rb.freezeRotation = true;
    }

    private void CheckY()
    {
        if (transform.position.y <= yDead) GameManager.instance.KillPlayer();
    }

    public void Die()
    {
        print("gamemanager says die");
        Player.instance.hasDied = true;
        animator.SetBool("hasDied", Player.instance.hasDied);
    }

    void Move()
    {
        // controls
        moveX = Input.GetAxis("Horizontal");
        animator.SetFloat("XSpeed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("YSpeed", rb.velocity.y);
        //if (Input.GetButtonDown("Horizontal")) ; // cool code to check button

        if (Input.GetButtonDown("Jump"))
        {
            if(Player.instance.isGrounded || Player.instance.canJump) Jump();
        }
        // animations
        // player direction
        if (moveX < 0.0f && !Player.instance.facingLeft) FlipPlayer();
        else if (moveX > 0.0f && Player.instance.facingLeft) FlipPlayer();
        // physics
        rb.velocity = new Vector2(
            moveX * Player.instance.speed,
            rb.velocity.y
        );
    }

    void FlipPlayer()
    {
        Player.instance.facingLeft = !Player.instance.facingLeft;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void GroundPlayer()
    {
        Player.instance.isGrounded = true;
        Player.instance.canJump = true;
        Player.instance.isHanging = false;
        animator.SetBool("isHanging", Player.instance.isHanging);
        animator.SetBool("isGrounded", true);
    }

    void Jump()
    {
        Player.instance.speed = Player.instance.maxSpeed;
        rb.gravityScale = 1;
        rb.AddForce(Vector2.up * Player.instance.jumpForce);
        Player.instance.isGrounded = false;
        Player.instance.isHanging = false;
        Player.instance.canJump = false;
        animator.SetBool("isGrounded", Player.instance.isGrounded);
        animator.SetBool("isHanging", Player.instance.isHanging);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // so using tilemaps, we can make new tilemaps and assign different tags to them, for ex: water and ground.
        //Debug.Log("player has collided with " + collision.collider.name + " with tag: " + collision.gameObject.tag);
        if (collision.gameObject.tag == "groundable") GroundPlayer();
        if (collision.gameObject.tag == "hangable" && !Player.instance.isGrounded) Hang();
      
        // this part isnt necessary since its easier to handle this collision as one collision within the enemycontroller
        if(collision.gameObject.tag.Contains("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (collision.gameObject.tag.Contains("Slime"))
            { 

            }
            Player.instance.TakeDamage(enemy.attributes.damage);
            if (Player.instance.health <= 0) GameManager.instance.KillPlayer();
        }
        
    }

    private void Hang()
    {
        Player.instance.isHanging = true;
        animator.SetBool("isHanging", Player.instance.isHanging);
        Player.instance.canJump = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Drop")) PickUpDrop(collision);
    }

    void PickUpDrop(Collider2D collision)
    {
        if (collision.tag.Contains("Coin"))
        {
            //play sound
            Player.instance.EatCoin();
            Destroy(collision.gameObject);
        }
        if (collision.tag.Contains("Music"))
        {
            //play sound
            Player.instance.EatMusic();
            Destroy(collision.gameObject);
        }
        if (collision.tag.Contains("Heart"))
        {
            //play sound
            Player.instance.EatHeart();
            Destroy(collision.gameObject);
        }
        if (collision.tag.Contains("Goal"))
        {
            //play sound
            if(GameManager.instance.enemies == 0)
            {
                Destroy(collision.gameObject);
                GameManager.instance.LoadNextStage();
            } 
                
        }
        
    }

    void PlayerRaycast()
    {
        // use this to kill enemies
        // every time this ray touches an enemy, bounce off his head
        RaycastHit2D rayDown = Physics2D.Raycast(
            transform.position,
            Vector2.down // shoot a ray down
        );
        // cast a ray from the top of the player
        RaycastHit2D rayUp = Physics2D.Raycast(
            transform.position,
            Vector2.up // shoot a ray down
        );
        // cast a ray to the left of the player
        RaycastHit2D rayLeft = Physics2D.Raycast(
            transform.position,
            Vector2.left // shoot a ray down
        );
        // cast a ray to the right of the player
        RaycastHit2D rayRight = Physics2D.Raycast(
            transform.position,
            Vector2.right // shoot a ray down
        );

        if (rayDown == false || rayDown.collider == null) return;

        if (rayDown.distance < 0.9f && rayDown.collider.tag.Contains("Enemy")) // kill enemy, hit enemy
        {
            EnemyController enemy = rayDown.collider.GetComponent<EnemyController>();
            if (rayDown.collider.tag.Contains("Slime") || rayDown.collider.tag.Contains("Skeleton"))
            {
                enemy.attributes.PlayHit();
            }
            Jump();
            enemy.TakeDamage();

        }
        if (rayDown.distance <= bottDist && rayDown.collider.tag == "hangable") // walk on top of hangable terrain
        {
            GroundPlayer();
        }

        //if(rayUp.distance < 0.9f && rayUp.collider.tag == "name")

    }

}
