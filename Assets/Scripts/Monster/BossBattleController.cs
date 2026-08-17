using UnityEngine;

public class BossBattleController : MonoBehaviour
{
    private const int RequiredSealCount = 4;

    private static BossBattleController instance;
    private Boss boss;
    private int miniBossesRemaining;
    private bool hasRemovedOtherMonsters;

    public static bool IsCleared => instance != null && instance.isCleared;
    private bool isCleared;

    public static BossBattleController GetOrCreate()
    {
        if (instance != null) return instance;

        instance = FindFirstObjectByType<BossBattleController>();
        if (instance != null) return instance;

        instance = new GameObject(nameof(BossBattleController)).AddComponent<BossBattleController>();
        return instance;
    }

    private void Update()
    {
        if (boss == null) boss = FindFirstObjectByType<Boss>();
        if (boss == null) return;

        if (boss.IsReleased)
        {
            if (!hasRemovedOtherMonsters)
            {
                RemoveOtherMonsters();
                hasRemovedOtherMonsters = true;
            }
            return;
        }

    }

    public bool AreAllSealsDefeated()
    {
        SealMonster[] seals = FindObjectsByType<SealMonster>(FindObjectsSortMode.None);
        if (seals.Length != RequiredSealCount) return false;

        foreach (SealMonster seal in seals)
        {
            if (!seal.IsReleased || !seal.monIsDead) return false;
        }

        return true;
    }

    public void SplitBoss(Boss source)
    {
        if (source != boss || miniBossesRemaining > 0) return;

        miniBossesRemaining = RequiredSealCount;
        for (int i = 0; i < miniBossesRemaining; i++)
        {
            float angle = i * Mathf.PI * 2f / miniBossesRemaining;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 2f;
            GameObject miniBossObject = Instantiate(source.gameObject, source.transform.position + offset, source.transform.rotation);
            Boss copiedBoss = miniBossObject.GetComponent<Boss>();
            copiedBoss.enabled = false;
            Destroy(copiedBoss);
            miniBossObject.AddComponent<MiniBoss>();
        }

        Destroy(source.gameObject);
    }

    public void ReportMiniBossDeath()
    {
        miniBossesRemaining--;
        if (miniBossesRemaining <= 0)
        {
            isCleared = true;
            Time.timeScale = 0f;
        }
    }

    private void RemoveOtherMonsters()
    {
        foreach (Monster monster in FindObjectsByType<Monster>(FindObjectsSortMode.None))
        {
            if (monster != boss) Destroy(monster.gameObject);
        }
    }
}
