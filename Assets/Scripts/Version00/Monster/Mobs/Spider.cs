using UnityEngine;

public class Spider : Monster
{
    private const float SurfaceOffset = 0.55f;
    private const float SurfaceProbeDistance = 1f;
    private const float SurfaceSearchDistance = 100f;
    private static readonly Vector3[] SurfaceProbeDirections =
    {
        Vector3.down, Vector3.up, Vector3.left, Vector3.right, Vector3.forward, Vector3.back
    };

    protected override MonsterType Type => MonsterType.spider;
    protected override bool UsesAI => true;
    protected override bool UsesGravity => false;
    protected override bool IsKinematic => true;

    protected override void ConfigureStats(int difficulty)
    {
        if (difficulty == 1) SetCombatStats(1, 1f, 5f, 50f);
        else if (difficulty == 2) SetCombatStats(5, 5f, 10f, 100f);
        else SetCombatStats(10, 10f, 20f, 200f);
    }

    protected override void OnDeath()
    {
        SpawnDeathMarkAndDestroy(100, 100);
    }

    protected override void TickMonster()
    {
        if (monIsDead) return;

        if (TryFindSurface(transform.position, SurfaceSearchDistance, out RaycastHit surface))
        {
            SnapToSurface(surface);
        }
    }

    public override void Move(Vector3 direction, float speed)
    {
        if (!TryFindSurface(transform.position, SurfaceProbeDistance, out RaycastHit surface)) return;

        Vector3 alongSurface = Vector3.ProjectOnPlane(direction, surface.normal);
        if (alongSurface.sqrMagnitude < Mathf.Epsilon)
        {
            SnapToSurface(surface);
            return;
        }

        float stepDistance = speed * Time.deltaTime;
        Vector3 stepDirection = alongSurface.normalized;

        if (Physics.Raycast(transform.position, stepDirection, out RaycastHit nextSurface,
                stepDistance + SurfaceOffset) && IsCrawlSurface(nextSurface))
        {
            SnapToSurface(nextSurface);
            return;
        }

        Vector3 nextPosition = transform.position + stepDirection * stepDistance;
        if (Physics.Raycast(nextPosition + surface.normal * SurfaceOffset, -surface.normal,
                out RaycastHit supportingSurface, SurfaceProbeDistance + SurfaceOffset) &&
            IsCrawlSurface(supportingSurface))
        {
            SnapToSurface(supportingSurface);
        }
    }

    private static bool TryFindSurface(Vector3 origin, float distance, out RaycastHit closestSurface)
    {
        closestSurface = default;
        float closestDistance = float.MaxValue;

        foreach (Vector3 probeDirection in SurfaceProbeDirections)
        {
            if (Physics.Raycast(origin, probeDirection, out RaycastHit hit, distance) &&
                IsCrawlSurface(hit) && hit.distance < closestDistance)
            {
                closestSurface = hit;
                closestDistance = hit.distance;
            }
        }

        return closestDistance < float.MaxValue;
    }

    private static bool IsCrawlSurface(RaycastHit hit)
    {
        return hit.collider != null && hit.collider.CompareTag("Ground");
    }

    private void SnapToSurface(RaycastHit surface)
    {
        // ponytail: Ground 태그 표면만 탐색한다. 복잡한 지형은 표면 그래프 또는 NavMesh 링크로 확장한다.
        MoveTo(surface.point + surface.normal * SurfaceOffset);
    }
}
