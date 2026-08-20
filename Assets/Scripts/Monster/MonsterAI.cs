using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    private const float AttackCooldown = 1f;

    private Monster monster;
    private Rigidbody rb;
    private Player player;
    private Transform playerTransform;
    private MonsterState state;
    private float detectRange;
    private float attackRange;
    private int attackForce;
    private float moveSpeed;
    private float idleMoveSpeed;
    private Vector3 idleDirection;
    private float nextIdleDirectionTime;
    private float nextAttackTime;
    private bool isInitialized;

    public void Initialize(Monster owner, Rigidbody rigidbody, Player target, float detectionRange, float attackDistance, int damage, float chaseSpeed, float idleSpeed)
    {
        monster = owner;
        rb = rigidbody;
        player = target;
        playerTransform = target.transform;
        detectRange = detectionRange;
        attackRange = attackDistance;
        attackForce = damage;
        moveSpeed = chaseSpeed;
        idleMoveSpeed = idleSpeed;
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
                MoveWhileIdle();
                if (targetDistance < detectRange) state = MonsterState.Chase;
                break;

            case MonsterState.Chase:
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                monster.Move(direction, moveSpeed);

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

    private void MoveWhileIdle()
    {
        if (idleMoveSpeed <= 0f) return;

        if (Time.time >= nextIdleDirectionTime)
        {
            idleDirection = Random.insideUnitSphere;
            idleDirection.y = 0f;
            idleDirection.Normalize();
            nextIdleDirectionTime = Time.time + 2f;
        }

        monster.Move(idleDirection, idleMoveSpeed);
    }
}
