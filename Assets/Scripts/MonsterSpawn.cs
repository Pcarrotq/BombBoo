using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] private GameObject monster;
    [SerializeField] private Transform cameraPivot;

    private int maxMonster;
    private int currMonster;
    private Vector3 randomPos;

    private float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        maxMonster = 20;
        currMonster = 0;

        spawnTime = 10f;
        InvokeRepeating(nameof(SpawnMonster), 1f, spawnTime);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SpawnMonster()
    {
        if (currMonster >= maxMonster) return;

        randomPos = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f));

        GameObject obj = Instantiate(monster, randomPos, Quaternion.identity);

        Monster m = obj.GetComponent<Monster>();
        m.SetCameraPivot(cameraPivot);

        currMonster++;
    }

    void DeadMonster()
    {
        currMonster--;
    }
}
