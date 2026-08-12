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
    private readonly List<Monster> spawnedMonsters = new List<Monster>();

    private float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        if (monster == null || cameraPivot == null)
        {
            Debug.LogError("Monster prefab or Camera Pivot is not assigned.", this);
            enabled = false;
            return;
        }

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
        spawnedMonsters.RemoveAll(spawnedMonster => spawnedMonster == null);
        currMonster = spawnedMonsters.Count;

        if (currMonster >= maxMonster) return;

        randomPos = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f));

        GameObject obj = Instantiate(monster, randomPos, Quaternion.identity);

        Monster m = obj.GetComponent<Monster>();
        if (m == null)
        {
            Debug.LogError("Monster prefab does not contain a Monster component.", obj);
            Destroy(obj);
            return;
        }

        m.SetCameraPivot(cameraPivot);
        spawnedMonsters.Add(m);
        currMonster = spawnedMonsters.Count;
    }
}
