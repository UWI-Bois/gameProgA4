using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public bool isClosed, exp, hp, score;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        isClosed = true;
        exp = false;
        hp = false;
        score = false;
        animator = GetComponent<Animator>();
        animator.SetBool("isClosed", isClosed);
        if (gameObject.tag.Contains("Wood")) score = true;
        if (gameObject.tag.Contains("Iron")) hp = true;
        if (gameObject.tag.Contains("Gold")) exp = true;
    }

    public void Open()
    {
        isClosed = false;
        animator.SetBool("isClosed", isClosed);
        AudioManager.instance.PlayChest();
        if (score) Player.instance.score += 10;
        if (hp) Player.instance.health += 10;
        if (exp) Player.instance.exp += 10;
    }
    

}
