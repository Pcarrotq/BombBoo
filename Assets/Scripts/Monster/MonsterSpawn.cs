using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] private Elemental elementalPrefab;
    [SerializeField] private Fish fishPrefab;
    [SerializeField] private Spider spiderPrefab;
    [SerializeField] private Tiger tigerPrefab;
    [SerializeField] private Transform cameraPivot;

    private int maxMonster;
    private int currMonster;
    private Vector3 randomPos;
    private readonly List<Monster> spawnedMonsters = new List<Monster>();

    private float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        if (elementalPrefab == null || fishPrefab == null || spiderPrefab == null || tigerPrefab == null || cameraPivot == null)
        {
            Debug.LogError("All monster prefabs and the Camera Pivot must be assigned.", this);
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

        Monster m = Instantiate(SelectPrefab(), randomPos, Quaternion.identity);
        m.SetCameraPivot(cameraPivot);
        spawnedMonsters.Add(m);
        currMonster = spawnedMonsters.Count;
    }

    private Monster SelectPrefab()
    {
        int roll = Random.Range(0, 100);

        if (roll < 50) return elementalPrefab;
        if (roll < 70) return fishPrefab;
        if (roll < 85) return spiderPrefab;
        return tigerPrefab;
    }
}
