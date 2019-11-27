using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttr : MonoBehaviour
{
    public int health, maxHealth, exp, toNextLevel, damage, score, level, lives, defaultLives;

    public static PlayerAttr playerAttr;

    private void Awake()
    {
        if (playerAttr == null) playerAttr = this;
        // check that it is equal to the current object
        else if (playerAttr != this)
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
    }
    
    override public string ToString()
    {
        return "hp: " + health
            + "\nexp: " + exp;
    }

}
