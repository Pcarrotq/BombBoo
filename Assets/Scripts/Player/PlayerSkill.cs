using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField] private Transform pAttackPoint;

    private readonly List<int> pAttackSkills = new List<int>();
    private readonly Vector3 pAttackRange = new Vector3(2f, 2f, 2f);
    private const float AttackForce = 10f;

    public int[] AttackSkillNumbers { get; private set; }

    void Awake()
    {
        AttackSkillNumbers = new int[3];
        for (int i = 1; i <= 10; i++)
        {
            pAttackSkills.Add(i);
        }
    }

    public void AttackRange()
    {
        if (pAttackPoint == null)
        {
            Debug.LogWarning("Attack point is not assigned.", this);
            return;
        }

        foreach (Collider collider in Physics.OverlapBox(pAttackPoint.position, pAttackRange))
        {
            Monster monster = collider.GetComponentInParent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(AttackForce);
            }
        }
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

        for (int i = 0; i < AttackSkillNumbers.Length; i++)
        {
            AttackSkillNumbers[i] = pAttackSkills[i];
            Debug.Log($"현재 스킬 {i + 1} : {AttackSkillNumbers[i]}");
        }

    }

    public void UseSkill(int skillNumber)
    {
        Debug.Log($"스킬 {skillNumber}");
        AttackSkill();
    }

    public void DestroyDeathMark()
    {
        if (pAttackPoint == null)
        {
            Debug.LogWarning("Attack point is not assigned.", this);
            return;
        }

        foreach (Collider collider in Physics.OverlapBox(pAttackPoint.position, pAttackRange))
        {
            if (collider.CompareTag("DeathMark"))
            {
                collider.GetComponent<MonsterDeathMark>()?.DestroyMark();
            }
        }
    }
}
