using System.Collections.Generic;
using UnityEngine;

public class Boss01 : MonoBehaviour
{
    private readonly HashSet<BossPond> ponds = new HashSet<BossPond>();
    private readonly HashSet<BossPond> activatedPonds = new HashSet<BossPond>();
    private readonly List<BossPond> activationOrder = new List<BossPond>();
    private BossHeartEnter01 heartEnter;
    private BossPond currentPond;

    public bool IsSealed => activatedPonds.Count < ponds.Count;
    public int RemainingPonds => ponds.Count - activatedPonds.Count;

    void Awake()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.isKinematic = true;
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezeAll;
        }

        heartEnter = GetComponentInChildren<BossHeartEnter01>() ?? gameObject.AddComponent<BossHeartEnter01>();
        foreach (BossPond pond in GetComponentsInChildren<BossPond>(true))
        {
            ponds.Add(pond);
        }
    }

    void Start()
    {
        Debug.Assert(ponds.Count == 4, $"Boss01 expects exactly four ponds, but found {ponds.Count}.", this);
        CameraController01 cameraController = FindFirstObjectByType<CameraController01>();
        activationOrder.AddRange(ponds);
        Shuffle(activationOrder);
        List<int> viewIndices = new List<int>();
        for (int i = 0; i < ponds.Count; i++) viewIndices.Add(i % 4);
        Shuffle(viewIndices);

        int index = 0;
        int restoredActiveCount = BossHeartThorn01.ChallengeStarted
            ? Mathf.Max(0, ponds.Count - BossHeartThorn01.OuterPondsToRestore)
            : 0;
        foreach (BossPond pond in activationOrder)
        {
            pond.Initialize(cameraController, viewIndices[index]);
            if (index++ >= restoredActiveCount) continue;
            pond.RestoreActivatedState();
            activatedPonds.Add(pond);
        }
        SetCurrentPond(restoredActiveCount < activationOrder.Count
            ? activationOrder[restoredActiveCount]
            : null);
        heartEnter.SetUnlocked(!IsSealed);
        if (BossHeartThorn01.IsComplete) CurseLump.BeginFinalPhase(transform);
    }

    public void RegisterPond(BossPond pond)
    {
        if (pond != null) ponds.Add(pond);
    }

    public void ActivatePond(BossPond pond)
    {
        if (pond != currentPond || !activatedPonds.Add(pond)) return;

        Debug.Log($"Boss seal activated. Remaining: {RemainingPonds}", this);
        SetCurrentPond(activatedPonds.Count < activationOrder.Count
            ? activationOrder[activatedPonds.Count]
            : null);
        if (!IsSealed)
        {
            BossHeartThorn01.ClearOuterRestore();
            heartEnter.SetUnlocked(true);
            Debug.Log("Boss01 seal released.", this);
        }
    }

    private void SetCurrentPond(BossPond pond)
    {
        currentPond?.SetCurrentTarget(false);
        currentPond = pond;
        currentPond?.SetCurrentTarget(true);
    }

    private static void Shuffle<T>(List<T> values)
    {
        for (int i = values.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (values[i], values[randomIndex]) = (values[randomIndex], values[i]);
        }
    }
}
