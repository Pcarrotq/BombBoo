using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    private const int WaveTargetScore = 500;

    [SerializeField] private Elemental elementalPrefab;
    [SerializeField] private Fish fishPrefab;
    [SerializeField] private Spider spiderPrefab;
    [SerializeField] private Tiger tigerPrefab;
    [SerializeField] private Transform cameraPivot;

    [SerializeField] private float elementalSpawnMinInterval = 3f;
    [SerializeField] private float elementalSpawnMaxInterval = 5f;

    public int TotalScore { get; private set; }
    public int WaveScore { get; private set; }
    public int WaveNumber { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (elementalPrefab == null || fishPrefab == null || spiderPrefab == null || tigerPrefab == null || cameraPivot == null)
        {
            Debug.LogError("All monster prefabs and the Camera Pivot must be assigned.", this);
            enabled = false;
            return;
        }

        StartNextWave();
    }

    public void AddScore(int score)
    {
        TotalScore += score;
        WaveScore += score;

        if (WaveScore >= WaveTargetScore)
        {
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        WaveNumber++;
        WaveScore = 0;
        Spawn(SelectSpecialMonsterPrefab());
        SpawnElemental();
        CancelInvoke(nameof(SpawnElemental));
        ScheduleNextElemental();
    }

    private void SpawnElemental()
    {
        Spawn(elementalPrefab);
        ScheduleNextElemental();
    }

    private void ScheduleNextElemental()
    {
        float minInterval = Mathf.Min(elementalSpawnMinInterval, elementalSpawnMaxInterval);
        float maxInterval = Mathf.Max(elementalSpawnMinInterval, elementalSpawnMaxInterval);
        Invoke(nameof(SpawnElemental), Random.Range(minInterval, maxInterval));
    }

    private void Spawn(Monster prefab)
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f));

        Monster m = Instantiate(prefab, randomPos, Quaternion.identity);
        m.SetCameraPivot(cameraPivot);
    }

    private Monster SelectSpecialMonsterPrefab()
    {
        return Random.Range(0, 3) switch
        {
            0 => fishPrefab,
            1 => spiderPrefab,
            _ => tigerPrefab
        };
    }
}
