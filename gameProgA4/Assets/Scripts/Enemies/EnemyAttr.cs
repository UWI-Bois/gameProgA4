using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttr : MonoBehaviour
{
    public int hp, expVal, scoreVal, damage;
    public int speed, jumpForce, fallSpeed, maxSpeed;
    public bool isStuck, isFalling, facingLeft, isGrounded, isDamaged, isDead, canRage;
    public int direction; // right = 1, left = -1;
    public float hitDist = 0.6f;
    public new string name;

    public AudioSource audioSource;
    public AudioClip hit, dead, enrage;
    // Start is called before the first frame update
    void Start()
    {
        hp = expVal = scoreVal = maxSpeed = speed = jumpForce = fallSpeed = damage = direction = 1;
        isStuck = isFalling = facingLeft = isGrounded = isDamaged = isDead = canRage = false;
        name = "enemy";
        audioSource = null; 
        hit = dead = enrage = null;
    }
    public void PlayHit()
    {
        if (hit != null) audioSource.PlayOneShot(hit);
    }
    public void PlayDead()
    {
        if (dead != null) audioSource.PlayOneShot(dead);
    }
    public void PlayEnrage()
    {
        if (enrage != null) audioSource.PlayOneShot(enrage);
    }

    public void Enrage()
    {
        hp += 12;
        damage += 2;
        speed += 2;
        expVal *= 2;
        canRage = false;
        PlayEnrage();
    }

    public string toString()
    {
        string s = "hp: " + hp
            + "\n expVal: " + expVal
            + "\n scoreVal: " + scoreVal
            + "\n speed: " + speed
            + "\n jumpForce: " + jumpForce;
        return s;
    }
}
