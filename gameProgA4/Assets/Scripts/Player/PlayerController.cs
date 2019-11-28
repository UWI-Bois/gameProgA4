using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // access ui elements
using UnityEngine.InputSystem;

public class PlayerController  : MonoBehaviour
{
    public PlayerControls controls; // joy con support

    public int yDead = -10;
    private float moveX;
    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 size;
    public Animator animator;

    public float bottDist;

    public AudioSource coinSound;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Gameplay.Jump.performed += context => Jump();
        //controls.Gameplay.Move.performed += context => size = context.ReadValue<Vector2>();
        //controls.Gameplay.Move.canceled += context => rb.velocity = Vector2.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        initController();
        //print(size.ToString() + col.ToString());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector2 m = new Vector2(size.x, size.y) * Time.deltaTime;
        //transform.Translate(m, Space.World);
        Move(); // move the player
        CheckY(); // check to see if you fell off the map
        PlayerRaycast(); // used to kill enemies
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
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

    void MoveJoyCon()
    {
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

    void Move()
    {
        // controls
        moveX = Input.GetAxis("Horizontal");
        //Input.GetAxis("Vertical");
        animator.SetFloat("XSpeed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("YSpeed", rb.velocity.y);
        //if (Input.GetButtonDown("Horizontal")) ; // cool code to check button

        if (Input.GetButtonDown("Jump"))
        {
             Jump();
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
        if (Player.instance.isGrounded || Player.instance.canJump || rb.velocity.y == 0 || Player.instance.isHanging)
        {
            Bounce();
        }    
    }

    void Bounce()
    {
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
        if (collision.gameObject.tag == "breakable" && !Player.instance.isGrounded) Hang();
      
        if(collision.gameObject.tag.Contains("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (collision.gameObject.tag.Contains("Slime"))
            { 

            }
            Player.instance.TakeDamage(enemy.attributes.damage);
            if (Player.instance.health <= 0) GameManager.instance.KillPlayer();
        }

        if (collision.gameObject.tag.Contains("Chest"))
        {
            ChestController cc = collision.gameObject.GetComponent<ChestController>();
            cc.Open();
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
            AudioManager.instance.PlayCoin();
            Player.instance.EatCoin();
            Destroy(collision.gameObject);
        }
        if (collision.tag.Contains("Music"))
        {
            AudioManager.instance.PlayMusic();
            Player.instance.EatMusic();
            Destroy(collision.gameObject);
        }
        if (collision.tag.Contains("Heart"))
        {
            AudioManager.instance.PlayHeart();
            Player.instance.EatHeart();
            Destroy(collision.gameObject);
        }
        if (collision.tag.Contains("Goal"))
        {
            AudioManager.instance.PlayGoal();
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
            Vector2.up // shoot a ray up
        );
        // cast a ray to the left of the player
        RaycastHit2D rayLeft = Physics2D.Raycast(
            transform.position,
            Vector2.left // shoot a ray left
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
            if (rayDown.collider.tag.Contains("Slime") || rayDown.collider.tag.Contains("Skeleton") || rayDown.collider.tag.Contains("Dio"))
            {
                //enemy.attributes.PlayHit();
            }
            Bounce();
            enemy.TakeDamage();

        }
        if (rayDown.distance <= bottDist && rayDown.collider.tag == "hangable") // walk on top of hangable terrain
        {
            GroundPlayer();
        }
        if (rayDown.distance <= bottDist && rayDown.collider.tag.Contains("Chest")) // walk on top of chests
        {
            GroundPlayer();
        }
        if (rayDown.distance <= bottDist && rayDown.collider.tag == "breakable") // break leaves
        {
            StartCoroutine(DestroyLeaves(rayDown.collider.gameObject));
        }
        //CheckChest(rayLeft, rayRight);
    }

    void CheckChest(RaycastHit2D rayLeft, RaycastHit2D rayRight)
    {
        bool hitChest = false;
        GameObject chestObject = null;
        ChestController cc = null;
        if (rayLeft.distance <= 0.3f && rayLeft.collider.tag.Contains("Chest"))
        {
            hitChest = true;
            chestObject = rayLeft.collider.gameObject;
            cc = chestObject.GetComponent<ChestController>();
        }
        else if (rayRight.distance <= 0.3f && rayRight.collider.tag.Contains("Chest"))
        {
            hitChest = true;
            chestObject = rayRight.collider.gameObject;
            cc = chestObject.GetComponent<ChestController>();
        }
        else return;
        if (!hitChest) return;
        else if (!cc.isClosed) return;
        else cc.Open();
    }

    private IEnumerator DestroyLeaves(GameObject go)
    {
        yield return new WaitForSeconds(2);
        AudioManager.instance.PlayLeaves();
        Destroy(go);
    }
}
