using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleController : MonoBehaviour
{
    private const int RequiredSealCount = 4;

    private static BossBattleController instance;
    private Boss boss;
    private int miniBossesRemaining;
    private bool hasRemovedOtherMonsters;
    private BossFogEffect fog;

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
        fog = gameObject.AddComponent<BossFogEffect>();
        List<MiniBoss> miniBosses = new List<MiniBoss>(miniBossesRemaining);
        for (int i = 0; i < miniBossesRemaining; i++)
        {
            float angle = i * Mathf.PI * 2f / miniBossesRemaining;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 2f;
            GameObject miniBossObject = Instantiate(source.gameObject, source.transform.position + offset, source.transform.rotation);
            Boss copiedBoss = miniBossObject.GetComponent<Boss>();
            copiedBoss.enabled = false;
            Destroy(copiedBoss);
            MiniBoss miniBoss = miniBossObject.AddComponent<MiniBoss>();
            miniBoss.HideInFog();
            miniBosses.Add(miniBoss);
        }

        Destroy(source.gameObject);
        StartCoroutine(RunAmbush(miniBosses));
    }

    private IEnumerator RunAmbush(List<MiniBoss> miniBosses)
    {
        yield return new WaitForSeconds(1f);

        foreach (MiniBoss miniBoss in miniBosses)
        {
            if (miniBoss == null) continue;

            miniBoss.AttackOnce();
            while (miniBoss != null && !miniBoss.HasFinishedAttack) yield return null;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ReportMiniBossDeath()
    {
        miniBossesRemaining--;
        if (miniBossesRemaining <= 0)
        {
            if (fog != null) Destroy(fog);
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

public class BossFogEffect : MonoBehaviour
{
    private const float TargetDensity = 0.06f;
    private static readonly Color FogColor = new Color(0.18f, 0.2f, 0.24f);

    private bool previousFog;
    private Color previousColor;
    private FogMode previousMode;
    private float previousDensity;

    private void Awake()
    {
        previousFog = RenderSettings.fog;
        previousColor = RenderSettings.fogColor;
        previousMode = RenderSettings.fogMode;
        previousDensity = RenderSettings.fogDensity;

        RenderSettings.fog = true;
        RenderSettings.fogColor = FogColor;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0f;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        const float duration = 1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Lerp(0f, TargetDensity, elapsed / duration);
            yield return null;
        }
    }

    private void OnDestroy()
    {
        RenderSettings.fog = previousFog;
        RenderSettings.fogColor = previousColor;
        RenderSettings.fogMode = previousMode;
        RenderSettings.fogDensity = previousDensity;
    }
}
