using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttr : EnemyAttr
{
    // Start is called before the first frame update
    
    void Start()
    {
        hp = 3;
        damage = 1;
        speed = maxSpeed = 3;
        expVal = scoreVal = 3;
        jumpForce = 200;
        fallSpeed = 3;
        direction = -1;
        facingLeft = isGrounded = true;
        name = "slime";

        audioSource = GetComponent<AudioSource>();

        //print(GetComponent<AudioSource>().name); // prints the name of the object (slime, slime1, slime2..)
        //print(GetComponent<AudioSource>().clip.name); // prints the name of the audio file supplied

        //print(gameObject.tag.ToString());
    }

    


}
