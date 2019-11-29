using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DioAttr : EnemyAttr
{
    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp = 60;
        damage = 10;
        speed = maxSpeed = 2;
        expVal = scoreVal = 5;
        jumpForce = 10;
        fallSpeed = 3;
        direction = 1;
        facingLeft = false; 
        isGrounded = canRage = true;
        enragedHP = maxHp / 2;
        name = "dio";
        layerNum = 10;

        audioSource = GetComponent<AudioSource>();
        //print(gameObject.tag.ToString());
    }


}
