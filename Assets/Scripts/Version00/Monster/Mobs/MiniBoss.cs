using System.Collections;
using UnityEngine;

public class MiniBoss : Monster
{
    private const float AmbushDistance = 6f;
    private const float AttackSpeed = 8f;
    private const float AttackTimeout = 3f;

    private Renderer[] renderers;
    private Collider[] colliders;
    private bool hasReported;

    public bool HasFinishedAttack { get; private set; }

    protected override MonsterType Type => MonsterType.miniboss;
    protected override bool UsesGravity => false;

    protected override void ConfigureStats(int difficulty)
    {
        SetCombatStats(5 * difficulty, 1.5f, 0f, 10f);
        miniBossNumMax = 4;
        miniBossNum = miniBossNumMax;
    }

    public void HideInFog()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
        SetVisible(false);
    }

    public void AttackOnce()
    {
        StartCoroutine(Ambush());
    }

    private IEnumerator Ambush()
    {
        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector3 targetPosition = Player.transform.position;
        transform.position = targetPosition + new Vector3(direction.x, 0f, direction.y) * AmbushDistance;
        SetVisible(true);

        float deadline = Time.time + AttackTimeout;
        while (!monIsDead && Time.time < deadline)
        {
            Vector3 offset = Player.transform.position - transform.position;
            offset.y = 0f;
            if (offset.magnitude <= AttackRange) break;

            Move(offset.normalized, AttackSpeed);
            yield return null;
        }

        if (!monIsDead && Player.playerType == PlayerType.bomb) Player.TakeDamage(AttackForce);
        FinishAttack();
    }

    private void SetVisible(bool visible)
    {
        if (renderers != null)
            foreach (Renderer targetRenderer in renderers) targetRenderer.enabled = visible;

        if (colliders != null)
            foreach (Collider targetCollider in colliders) targetCollider.enabled = visible;
    }

    protected override void OnDeath()
    {
        FinishAttack();
    }

    private void FinishAttack()
    {
        if (hasReported) return;

        hasReported = true;
        HasFinishedAttack = true;
        SetVisible(false);
        BossBattleController.GetOrCreate().ReportMiniBossDeath();
        Destroy(gameObject);
    }
}
