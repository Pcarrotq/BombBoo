using System.Collections.Generic;
using UnityEngine;

public class Player01 : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Rigidbody rb;
    private Animator animator;
    private readonly HashSet<Collider> groundContacts = new HashSet<Collider>();
    private Vector3 moveDirection;
    private bool isGround;
    private int absorptionAmount;

    private float pMaxHP;
    private float pCurrHP;

    [SerializeField] private Transform cameraPivot;
    private float moveSpeed = 5f;
    private float jumpForce = 5f;
    
    [SerializeField] private Transform pAttackPoint;
    private float attackRadius = 1f;
    private const float AttackForce = 10f;

    private float nextDamageTime;
    private const float DamageCooldown = 0.5f;

    public PlayerType playerType;
    private float booTimer;
    private int pCurrExp;
    private int pAbsorption;
    private int pAbsorptionLimit;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator = GetComponent<Animator>();
        
        pMaxHP = 100f;
        pCurrHP = pMaxHP;

        playerType = PlayerType.bomb;
        booTimer = 5f;
        absorptionAmount = 10;
        pAbsorptionLimit = 100;
    }

    void Update()
    {
        moveDirection = Vector3.zero;
        KeyInput();
        transform.rotation = cameraPivot.rotation;
    }

    void FixedUpdate()
    {
        if (moveDirection != Vector3.zero)
        {
            rb.MovePosition(rb.position + moveDirection.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;

        groundContacts.Add(collision.collider);
        UpdateGroundedState();
        animator.SetBool("IsJump", false);
    }

    void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;

        groundContacts.Remove(collision.collider);
        UpdateGroundedState();
    }

    private void UpdateGroundedState()
    {
        isGround = groundContacts.Count > 0;
        animator.SetBool("IsGround", isGround);
    }

    private void KeyInput()
    {
        animator.SetBool("isMoving", false);

        if (playerType == PlayerType.bomb)
        {
            Vector3 cameraRight = Vector3.ProjectOnPlane(cameraPivot.right, Vector3.up).normalized;

            if (Input.GetKey(KeyCode.A))
            {
                sprite.flipX = false;
                animator.SetBool("isMoving", true);
                moveDirection -= cameraRight;
            }
            if (Input.GetKey(KeyCode.D))
            {
                sprite.flipX = true;
                animator.SetBool("isMoving", true);
                moveDirection += cameraRight;
            }
            if (Input.GetKey(KeyCode.Space) && isGround)
            {
                animator.SetBool("isJumpReady", true);
            }
            if (Input.GetKeyUp(KeyCode.Space) && isGround)
            {
                animator.SetBool("isJumpReady", false);
                animator.SetBool("IsJump", true);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                AttackRange();
            }
        }
        else
        {
            booTimer -= Time.deltaTime;
            if (booTimer <= 0f)
            {
                SetPlayerType(PlayerType.bomb);
                return;
            }

            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("isMoving", true);
                moveDirection += cameraPivot.up;
            }
            if (Input.GetKey(KeyCode.A))
            {
                sprite.flipX = false;
                animator.SetBool("isMoving", true);
                moveDirection -= cameraPivot.right;
            }
            if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool("isMoving", true);
                moveDirection -= cameraPivot.up;
            }
            if (Input.GetKey(KeyCode.D))
            {
                sprite.flipX = true;
                animator.SetBool("isMoving", true);
                moveDirection += cameraPivot.right;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (DestroyDeathMark())
                {
                    pCurrExp = Mathf.Max(0, pCurrExp - 5);
                    pAbsorption += absorptionAmount;
                    if (pAbsorption >= pAbsorptionLimit)
                    {
                        SelfDestruct();
                        pAbsorption = 0;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetPlayerType(playerType == PlayerType.bomb ? PlayerType.boo : PlayerType.bomb);
        }
    }

    private static float RollDamage(float value) { return Random.Range(Mathf.Max(0f, value - 10f), value + 10f); }

    public bool DestroyDeathMark()
    {
        if (!TryGetAttackPoint()) return false;
        bool destroyed = false;
        foreach (Collider collider in GetAttackHits())
        {
            if (!collider.CompareTag("DeathMark")) continue;
            MonsterDeathMark deathMark = collider.GetComponent<MonsterDeathMark>();
            if (deathMark == null) continue;
            deathMark.DestroyMark();
            destroyed = true;
        }
        return destroyed;
    }

    public void SelfDestruct()
    {
        Damage(Physics.OverlapSphere(transform.position, 34f), RollDamage(250f));
        TakeDamage(RollDamage(250f), true);
    }

    public void TakeDamage(float damage, bool ignoreCooldown = false)
    {
        if (pCurrHP <= 0 || (!ignoreCooldown && Time.time < nextDamageTime)) return;

        nextDamageTime = Time.time + DamageCooldown;
        pCurrHP -= damage;
        Debug.Log("Player " + damage + "Damage!");

        if (pCurrHP <= 0)
        {
            Time.timeScale = 0f;
            pCurrExp = 0;
        }
    }

    public void AttackRange()
    {
        if (!TryGetAttackPoint()) return;

        Collider[] hits = GetAttackHits();
        Damage(hits, AttackForce);

        HashSet<BossPond> ponds = new HashSet<BossPond>();
        HashSet<BossHeartEnter01> entrances = new HashSet<BossHeartEnter01>();
        foreach (Collider hit in hits)
        {
            BossPond pond = hit.GetComponentInParent<BossPond>();
            if (pond != null && ponds.Add(pond)) pond.TryActivate();

            BossHeartEnter01 entrance = hit.GetComponentInParent<BossHeartEnter01>();
            if (entrance != null && entrances.Add(entrance)) entrance.TryEnter();
        }
    }

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

    private Collider[] GetAttackHits()
    {
        return Physics.OverlapCapsule(transform.position, pAttackPoint.position, attackRadius);
    }

    private void SetPlayerType(PlayerType type)
    {
        bool isBoo = type == PlayerType.boo;
        playerType = type;
        rb.useGravity = !isBoo;
        animator.SetBool("isBoo", isBoo);
        animator.SetBool("isJumpReady", false);

        if (!isBoo)
        {
            booTimer = 5f;
        }
    }
}
