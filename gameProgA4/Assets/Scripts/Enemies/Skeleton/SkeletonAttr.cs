using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttr : EnemyAttr
{
    // Start is called before the first frame update
    void Start()
    {
        hp = 14;
        damage = 2;
        speed = maxSpeed = 2;
        expVal = scoreVal = 5;
        jumpForce = 50;
        fallSpeed = 3;
        direction = 1;
        facingLeft = false; 
        isGrounded = canRage = true;
        enragedHP = hp / 2;
        name = "skele";
        layerNum = 13;

        audioSource = GetComponent<AudioSource>();
        //print(gameObject.tag.ToString());
    }


}
