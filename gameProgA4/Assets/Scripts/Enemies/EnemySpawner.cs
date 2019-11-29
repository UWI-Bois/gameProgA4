using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawn1, spawn2;
    public GameObject Enemy1, Enemy2;
    public float timer, timerMax;
    public bool canSpawn;
    // Start is called before the first frame update
    void Start()
    {
        timer = timerMax = 5f;
        canSpawn = false;
        spawn1 = gameObject.transform.Find("spawn1");
        spawn2 = gameObject.transform.Find("spawn2");
    }

    // Update is called once per frame
    void Update()
    {
        CheckTimer();
        //print(GameManager.instance.timeElapsed);
    }

    void CheckTimer()
    {
        timer -= Time.deltaTime;
        if ((int)timer == 0)
        {
            canSpawn = true;
            timer = timerMax;
            Spawn();
        }
        else canSpawn = false;
    }

    public void Spawn()
    {
        if (!canSpawn) return;
        print("tryna spawn");
        Instantiate(Enemy1, spawn1.transform.InverseTransformVector(spawn1.position), Quaternion.identity);
        Instantiate(Enemy2, spawn1.transform.InverseTransformVector(spawn2.position), Quaternion.identity);

    }
}
