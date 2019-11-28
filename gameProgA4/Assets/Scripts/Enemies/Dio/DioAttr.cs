using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DioAttr : EnemyAttr
{
    // Start is called before the first frame update
    void Start()
    {
        hp = 30;
        damage = 3;
        speed = maxSpeed = 2;
        expVal = scoreVal = 5;
        jumpForce = 0;
        fallSpeed = 3;
        direction = 1;
        facingLeft = false; 
        isGrounded = canRage = true;
        name = "dio";

        audioSource = GetComponent<AudioSource>();
        //print(gameObject.tag.ToString());
    }


}
