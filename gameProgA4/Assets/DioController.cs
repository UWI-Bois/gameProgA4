using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DioController : MonoBehaviour
{
    EnemyController ec;
    // Start is called before the first frame update
    void Start()
    {
        ec = GetComponent<EnemyController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Contains("Skeleton") && ec.attributes.isEnraged) EatFriend(collision.gameObject);
    }

    public void EatFriend(GameObject enemy)
    {
        if (!ec.attributes.isEnraged || ec.attributes.hp >= ec.attributes.enragedHP) return;
        EnemyController skeleController = enemy.GetComponent<EnemyController>();
        ec.Attack();
        int gain = skeleController.attributes.hp + 1;
        if (gain > 0) ec.attributes.hp += gain;
        else ec.attributes.hp += 3;
        print("dio gained " + gain);
        skeleController.GetEaten(ec.attributes.damage);
        // play sound
    }
}
