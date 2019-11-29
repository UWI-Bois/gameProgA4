using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttr : MonoBehaviour
{
    public int hp, expVal, scoreVal, damage, enragedHP, layerNum;
    public int speed, jumpForce, fallSpeed, maxSpeed;
    public bool isStuck, isFalling, facingLeft, isGrounded, isDamaged, isDead, canRage, isEnraged, wasEaten;
    public int direction; // right = 1, left = -1;
    public float hitDist = 0.6f;
    public new string name;

    public AudioSource audioSource;
    public AudioClip hit, dead, enrage, eat;
    // Start is called before the first frame update
    void Start()
    {
        layerNum = 0;
        hp = expVal = scoreVal = maxSpeed = speed = jumpForce = fallSpeed = enragedHP = damage = direction = 1;
        isStuck = isFalling = facingLeft = isGrounded = isDamaged = isDead = canRage = isEnraged = wasEaten = false;
        name = "enemy";
        audioSource = null; 
        hit = dead = enrage = null;

        // new defaults
        jumpForce = 50;
    }
    public void PlayHit()
    {
        if (hit != null)
        {
            if (audioSource.clip == hit) return;
            audioSource.PlayOneShot(hit);
        }
        
    }
    public void PlayDead()
    {
        if (dead != null)
        {
            if (audioSource.clip == dead) return;
            audioSource.PlayOneShot(dead);
        }
    }
    public void PlayEnrage()
    {
        if (enrage != null)
        {
            if (audioSource.clip == enrage) return;
            audioSource.PlayOneShot(enrage);
        }
    }
    public void PlayEat()
    {
        if (eat != null)
        {
            if (audioSource.clip == eat) return;
            audioSource.PlayOneShot(eat);
        }
    }

    public void Enrage()
    {
        if (isEnraged) return;
        hp += hp/2;
        damage += 2;
        speed += 2;
        expVal *= 2;
        canRage = false;
        isEnraged = true;
        //print("enraged!");
        PlayEnrage();
    }

    public string toString()
    {
        string s = "hp: " + hp
            + "\n layerNum: " + layerNum
            + "\n expVal: " + expVal
            + "\n scoreVal: " + scoreVal
            + "\n speed: " + speed
            + "\n jumpForce: " + jumpForce;
        return s;
    }
}
