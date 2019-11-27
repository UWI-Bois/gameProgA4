using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttr : MonoBehaviour
{
    public int health, maxHealth, exp;

    public void ResetStats()
    {
        health = maxHealth = 4;
        exp = 0;
    }
    
    override public string ToString()
    {
        return "hp: " + health
            + "\nexp: " + exp;
    }

}
