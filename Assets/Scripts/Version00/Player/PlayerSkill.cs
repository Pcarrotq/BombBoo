using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField] private Transform pAttackPoint;
    private readonly List<int> pAttackSkills = new List<int>();
    private readonly Vector3 pAttackRange = new Vector3(2f, 2f, 2f);
    private const float AttackForce = 10f;
    private Player player;
    private Rigidbody playerBody;
    public int[] AttackSkillNumbers { get; private set; }

    void Awake()
    {
        player = GetComponent<Player>();
        playerBody = GetComponent<Rigidbody>();
        AttackSkillNumbers = new int[3];
        for (int i = 1; i <= 10; i++) pAttackSkills.Add(i);
    }

    public void AttackRange()
    {
        if (TryGetAttackPoint()) Damage(Physics.OverlapBox(pAttackPoint.position, pAttackRange), AttackForce);
    }

    public void AttackSkill()
    {
        for (int i = 0; i < pAttackSkills.Count; i++)
        {
            int randomSkillIndex = Random.Range(i, pAttackSkills.Count);
            int selectedSkill = pAttackSkills[i];
            pAttackSkills[i] = pAttackSkills[randomSkillIndex];
            pAttackSkills[randomSkillIndex] = selectedSkill;
        }
        for (int i = 0; i < AttackSkillNumbers.Length; i++) AttackSkillNumbers[i] = pAttackSkills[i];
    }

    public void UseSkill(int skillNumber)
    {
        if (!TryGetAttackPoint()) return;
        switch (skillNumber)
        {
            case 1: FallingSmash(); break;
            case 2: StartCoroutine(RampageCharge()); break;
            case 3: Damage(Physics.OverlapSphere(transform.position, 12f), RollDamage(60f)); break;
            case 4: CaptureShot(); break;
            case 5: SparkBarrage(); break;
            case 6: LeapAssault(); break;
            case 7: EmberBomb(); break;
            case 8: GroundSmash(); break;
            case 9: Backfire(); break;
            case 10: GunpowderZone(); break;
            default: Debug.LogWarning($"Unknown skill: {skillNumber}", this); break;
        }
        AttackSkill();
    }

    private void FallingSmash()
    {
        float fallBonus = playerBody != null ? Mathf.Max(0f, -playerBody.velocity.y) * 10f : 0f;
        Collider[] hits = Physics.OverlapSphere(transform.position, 8f);
        Damage(hits, RollDamage(8f));
        foreach (Monster monster in UniqueMonsters(hits))
        {
            monster.TakeDamage(RollDamage(100f + fallBonus));
            monster.ApplyDamageOverTime(RollDamage(10f), RollTime(5f));
        }
    }

    private IEnumerator RampageCharge()
    {
        float end = Time.time + 4f;
        HashSet<Monster> hit = new HashSet<Monster>();
        while (Time.time < end)
        {
            if (playerBody != null) playerBody.MovePosition(playerBody.position + transform.forward * 12f * Time.fixedDeltaTime);
            foreach (Monster monster in UniqueMonsters(Physics.OverlapSphere(transform.position, 1f)))
                if (hit.Add(monster)) { monster.TakeDamage(RollDamage(45f)); monster.Stun(RollTime(1.5f)); }
            yield return new WaitForFixedUpdate();
        }
        if (playerBody != null) playerBody.AddForce(transform.forward * 2f, ForceMode.VelocityChange);
    }

    private void CaptureShot()
    {
        Monster monster;
        if (!TryRaycastMonster(14f, out monster)) return;
        monster.ApplyDamageOverTime(RollDamage(16f), RollTime(1.5f));
        monster.Stun(RollTime(1.5f));
        monster.PullTo(transform.position);
    }

    private void SparkBarrage()
    {
        foreach (Monster monster in FindObjectsByType<Monster>(FindObjectsSortMode.None))
        {
            monster.TakeDamage(RollDamage(50f));
            monster.Stun(RollTime(3f));
        }
    }

    private void LeapAssault()
    {
        Monster monster;
        if (!TryRaycastMonster(30f, out monster)) return;
        transform.position = monster.transform.position - transform.forward;
        monster.TakeDamage(RollDamage(170f));
        monster.Stun(RollTime(1.9f));
    }

    private void EmberBomb()
    {
        Monster target;
        if (TryRaycastMonster(14f, out target, .5f)) StartCoroutine(DetonateEmber(target));
    }

    private IEnumerator DetonateEmber(Monster target)
    {
        yield return new WaitForSeconds(RollTime(2f));
        if (target == null) yield break;
        Vector3 position = target.transform.position;
        target.TakeDamage(float.MaxValue);
        Damage(Physics.OverlapSphere(position, 8f), RollDamage(63f), target);
    }

    private void GroundSmash()
    {
        Damage(Physics.OverlapBox(pAttackPoint.position + transform.forward * 7f,
            new Vector3(1f, 2f, 7f), transform.rotation), RollDamage(123f));
    }

    private void Backfire()
    {
        Monster monster;
        if (!TryRaycastMonster(5f, out monster)) return;
        monster.TakeDamage(RollDamage(115f));
        monster.Stun(RollTime(2f));
    }

    private void GunpowderZone()
    {
        Damage(Physics.OverlapSphere(transform.position, 11f), RollDamage(120f));
        player.TakeDamage(RollDamage(120f), true);
    }

    public void SelfDestruct()
    {
        Damage(Physics.OverlapSphere(transform.position, 34f), RollDamage(250f));
        player.TakeDamage(RollDamage(250f), true);
    }

    private bool TryRaycastMonster(float range, out Monster monster, float radius = 0f)
    {
        RaycastHit hit;
        bool didHit = radius > 0f
            ? Physics.SphereCast(pAttackPoint.position, radius, transform.forward, out hit, range)
            : Physics.Raycast(pAttackPoint.position, transform.forward, out hit, range);
        monster = didHit ? hit.collider.GetComponentInParent<Monster>() : null;
        return monster != null;
    }

    private static float RollDamage(float value) { return Random.Range(Mathf.Max(0f, value - 10f), value + 10f); }
    private static float RollTime(float value) { return Random.Range(Mathf.Max(0f, value - .5f), value + .5f); }

    private static IEnumerable<Monster> UniqueMonsters(Collider[] colliders)
    {
        HashSet<Monster> monsters = new HashSet<Monster>();
        foreach (Collider collider in colliders)
        {
            Monster monster = collider.GetComponentInParent<Monster>();
            if (monster != null && monsters.Add(monster)) yield return monster;
        }
    }

    private static void Damage(Collider[] colliders, float damage, Monster excluded = null)
    {
        foreach (Monster monster in UniqueMonsters(colliders)) if (monster != excluded) monster.TakeDamage(damage);
    }

    private bool TryGetAttackPoint()
    {
        if (pAttackPoint != null) return true;
        Debug.LogWarning("Attack point is not assigned.", this);
        return false;
    }

    public bool DestroyDeathMark()
    {
        if (!TryGetAttackPoint()) return false;
        bool destroyed = false;
        foreach (Collider collider in Physics.OverlapBox(pAttackPoint.position, pAttackRange))
        {
            if (!collider.CompareTag("DeathMark")) continue;
            MonsterDeathMark deathMark = collider.GetComponent<MonsterDeathMark>();
            if (deathMark == null) continue;
            deathMark.DestroyMark();
            destroyed = true;
        }
        return destroyed;
    }
}
