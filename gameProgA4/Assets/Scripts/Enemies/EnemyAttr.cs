using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttr : MonoBehaviour
{
    public int hp = 2;
    public int expVal, scoreVal;
    public int speed;
    // Start is called before the first frame update
    void Start()
    {
        hp = expVal = scoreVal = speed = 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    public string toString()
    {
        string s = "hp: " + hp
            + "\n expVal: " + expVal
            + "\n scoreVal: " + scoreVal
            + "\n speed: " + speed;
        return s;
    }
}
