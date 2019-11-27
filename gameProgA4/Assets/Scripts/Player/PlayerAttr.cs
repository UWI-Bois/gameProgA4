using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttr : MonoBehaviour
{
    public int health, maxHealth, exp, toNextLevel, damage, score, level, lives, defaultLives, jumpForce, speed, maxSpeed;
    public bool isGrounded, isHanging, canJump, hasDied, facingLeft;

    public static PlayerAttr instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        // check that it is equal to the current object
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //prevent this object from being destroyed when switching scenes
        DontDestroyOnLoad(gameObject);
    }

    public void ResetStats()
    {
        health = maxHealth = 4;
        level = damage = 1;
        toNextLevel = 10;
        lives = defaultLives = 3;
        score = exp = 0;
        canJump = isHanging = facingLeft = hasDied = isGrounded = false;
        jumpForce = 350;
        speed = maxSpeed = 6;
    }
    
    override public string ToString()
    {
        return "hp: " + health
            + "\nexp: " + exp;
    }

}
