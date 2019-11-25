using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttr : EnemyAttr
{
    // Start is called before the first frame update
    void Start()
    {
        hp = 2;
        speed = 3;
        expVal = scoreVal = 3;
        print(gameObject.tag.ToString());
    }


}
