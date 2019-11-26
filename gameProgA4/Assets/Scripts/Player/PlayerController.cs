using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // access ui elements

public class PlayerController  : MonoBehaviour
{
    public int jumpForce;
    public int speed, maxSpeed;
    public int yDead = -10;

    public bool facingLeft;
    public bool hasDied;
    public bool isGrounded, isHanging, canJump;

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
        initPlayer();
        //print(size.ToString() + col.ToString());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move(); // move the player
        CheckY(); // check to see if you fell off the map
        PlayerRaycast(); // used to kill enemies
    }

    void initPlayer()
    {
        DataManagement.dataManagement.LoadData();
        canJump = isHanging = facingLeft = hasDied = isGrounded = false;
        bottDist = 0.5f;
        maxSpeed = 6;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        jumpForce = 350;
        speed = maxSpeed;
        size = col.bounds.size;
        rb.freezeRotation = true;
    }

    private void CheckY()
    {
        // this function will check the y value of the player, as well as the y velocity
        //if ((Vector3)rb.velocity == Vector3.zero) isGrounded = true; 
        
        if (transform.position.y <= yDead) GameManager.instance.Die();
    }

    private IEnumerator Die()
    {
        hasDied = true;
        animator.SetBool("hasDied", hasDied);
        yield return new WaitForSeconds(1.5f);
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
            if(isGrounded || canJump) Jump();
        }
        // animations
        // player direction
        if (moveX < 0.0f && !facingLeft) FlipPlayer();
        else if (moveX > 0.0f && facingLeft) FlipPlayer();
        // physics
        rb.velocity = new Vector2(
            moveX * speed,
            rb.velocity.y
        );
    }

    void FlipPlayer()
    {
        facingLeft = !facingLeft;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void GroundPlayer()
    {
        isGrounded = true;
        canJump = true;
        isHanging = false;
        animator.SetBool("isHanging", isHanging);
        animator.SetBool("isGrounded", true);
    }

    void Jump()
    {
        speed = maxSpeed;
        rb.gravityScale = 1;
        rb.AddForce(Vector2.up * jumpForce);
        isGrounded = false;
        isHanging = false;
        canJump = false;
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isHanging", isHanging);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // so using tilemaps, we can make new tilemaps and assign different tags to them, for ex: water and ground.
        //Debug.Log("player has collided with " + collision.collider.name + " with tag: " + collision.gameObject.tag);
        if (collision.gameObject.tag == "groundable") GroundPlayer();
        if (collision.gameObject.tag == "hangable" && !isGrounded) Hang();
      
        if(collision.gameObject.tag.Contains("Enemy"))
        {
            if (collision.gameObject.tag.Contains("Slime"))
            {
                print("bonx a slime!");
                // take damage load an anim
            }
        }
        
    }

    private void Hang()
    {
        isHanging = true;
        animator.SetBool("isHanging", isHanging);
        canJump = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Drop_Goal")) GameManager.instance.LoadNextStage();
        if(collision.CompareTag("Drop_Coin")) EatCoin(collision);
        if(collision.CompareTag("Drop_Heart")) EatHeart(collision);
        if(collision.CompareTag("Drop_Music")) EatMusic(collision);
    }

    private void EatHeart(Collider2D collision)
    {
        if (!GameManager.instance.isDamaged()) return; // if player is not damage, back out 
        GameManager.instance.EatHeart();
        Destroy(collision.gameObject);
    }

    private void EatCoin(Collider2D collision)
    {
        //print("eating coin");
        GameManager.instance.EatCoin();
        // play a sound
        Destroy(collision.gameObject);
    }
    private void EatMusic(Collider2D collision)
    {
        //print("eating coin");
        GameManager.instance.EatMusicNote();
        // play a sound
        Destroy(collision.gameObject);
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
            if (rayDown.collider.tag.Contains("Slime"))
            {
                Jump();
                EnemyController e = rayDown.collider.GetComponent<EnemyController>();
                e.TakeDamage();
            }
            
        }
        if (rayDown.distance <= bottDist && rayDown.collider.tag == "hangable") // walk on top of hangable terrain
        {
            GroundPlayer();
        }

        //if(rayUp.distance < 0.9f && rayUp.collider.tag == "name")

    }

}
