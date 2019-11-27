using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health, maxHealth, exp, toNextLevel, damage, score, level, lives, defaultLives, jumpForce, speed, maxSpeed;
    public bool isGrounded, isHanging, canJump, hasDied, facingLeft;

    public static Player instance;

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

    public void TakeDamage(int amt)
    {
        health -= amt;
    }
    public void EatCoin()
    {
        score += GameManager.instance.coinVal;
        GameManager.instance.levelScore += GameManager.instance.coinVal;
        //if (hudManager != null) hudManager.ResetHUD();
        if (score > GameManager.instance.highScore) GameManager.instance.highScore = score;
    }
    public void EatMusic()
    {
        IncreaseEXP(GameManager.instance.musicNoteVal);
        GameManager.instance.timeLeft += GameManager.instance.musicNoteVal + 2;
        GameManager.instance.timeElapsed -= GameManager.instance.musicNoteVal + 2;
    }
    public void EatHeart()
    {
        IncreaseHP(GameManager.instance.heartVal);
    }
    public void KillSlime()
    {
        IncreaseEXP(GameManager.instance.slimeVal);
    }
    public bool isDamaged()
    {
        if (health < maxHealth) return true;
        return false;
    }
    public void IncreaseEXP(int amt)
    {
        exp += amt;
        if (exp >= toNextLevel) LevelUp();
        //if (hudManager != null) hudManager.ResetHUD();
    }
    public void IncreaseHP(int amt)
    {
        health += amt;
    }
    private void LevelUp()
    {
        int diff = toNextLevel - exp;
        level++;
        if (level % 2 == 0) maxHealth++;
        else if (level % 5 == 0)
        {
            maxHealth++;
            damage++;
        }
        else damage++;
        health = maxHealth;
        exp += diff;
        toNextLevel = toNextLevel + 10;
        //if (hudManager != null) hudManager.ResetHUD();
        // probably add an effect here? sound
    }
}
