using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    private const float MoveSpeed = 2f;
    private const float AttackCooldown = 1f;

    private Monster monster;
    private Rigidbody rb;
    private Player player;
    private Transform playerTransform;
    private MonsterState state;
    private float detectRange;
    private float attackRange;
    private int attackForce;
    private float nextAttackTime;
    private bool isInitialized;

    public void Initialize(Monster owner, Rigidbody rigidbody, Player target, float detectionRange, float attackDistance, int damage)
    {
        monster = owner;
        rb = rigidbody;
        player = target;
        playerTransform = target.transform;
        detectRange = detectionRange;
        attackRange = attackDistance;
        attackForce = damage;
        state = MonsterState.Idle;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || !monster.CanRunAI) return;

        float targetDistance = Vector3.Distance(transform.position, playerTransform.position);

        switch (state)
        {
            case MonsterState.Idle:
                if (targetDistance < detectRange) state = MonsterState.Chase;
                break;

            case MonsterState.Chase:
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                rb.MovePosition(rb.position + direction * Time.deltaTime * MoveSpeed);

                if (targetDistance > detectRange) state = MonsterState.Idle;
                else if (targetDistance < attackRange) state = MonsterState.Attack;
                break;

            case MonsterState.Attack:
                if (targetDistance < attackRange && player.playerType == PlayerType.bomb)
                {
                    if (Time.time >= nextAttackTime)
                    {
                        player.TakeDamage(attackForce);
                        nextAttackTime = Time.time + AttackCooldown;
                    }
                }
                else
                {
                    state = MonsterState.Chase;
                }
                break;
        }
    }
}
