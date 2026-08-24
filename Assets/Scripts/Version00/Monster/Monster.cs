using System.Collections;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    public MonsterType monsterType;
    public float mCurHP;
    public float mMaxHP;
    public bool monIsDead;
    [SerializeField] private MonsterDeathMark deathMark;

    public int miniBossNum;
    public int miniBossNumMax;

    private Rigidbody rb;
    private Player player;
    private Transform playerTransform;
    [SerializeField] private Transform cameraPivot;
    private bool isReleased;
    private float stunnedUntil;

    protected Player Player => player;
    protected virtual bool UsesAI => false;
    protected virtual bool StartsSealed => false;
    protected virtual bool UsesGravity => true;
    protected virtual bool IsKinematic => false;
    protected virtual float ChaseMoveSpeed => 2f;
    protected virtual float IdleMoveSpeed => 0.5f;
    protected virtual float IdleMoveChance => 0.5f;
    public bool CanRunAI => !monIsDead && Time.time >= stunnedUntil && UsesAI && (!StartsSealed || isReleased);
    public bool IsReleased => isReleased;

    protected abstract MonsterType Type { get; }

    protected virtual void Start()
    {
        monsterType = Type;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Monster requires a Rigidbody.", this);
            enabled = false;
            return;
        }

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.isKinematic = IsKinematic;
        rb.useGravity = UsesGravity;
        ConfigureStats(GameManager.Instance != null ? Mathf.Clamp(GameManager.Instance.diffIndex, 1, 3) : 1);
        mCurHP = mMaxHP;

        player = FindFirstObjectByType<Player>();
        if (cameraPivot == null)
        {
            CameraController cameraController = FindFirstObjectByType<CameraController>();
            cameraPivot = cameraController != null ? cameraController.CameraPivot : null;
        }

        if (player == null || cameraPivot == null)
        {
            Debug.LogError("Player or Camera Pivot is not assigned.", this);
            enabled = false;
            return;
        }

        playerTransform = player.transform;

        if (StartsSealed)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        MonsterAI monsterAI = GetComponent<MonsterAI>() ?? gameObject.AddComponent<MonsterAI>();
        monsterAI.Initialize(
            this,
            rb,
            player,
            DetectRange,
            AttackRange,
            AttackForce,
            ChaseMoveSpeed,
            IdleMoveSpeed,
            IdleMoveChance);
    }

    protected int AttackForce { get; private set; }
    protected float AttackRange { get; private set; }
    protected float DetectRange { get; private set; }

    protected void SetCombatStats(int attackForce, float attackRange, float detectRange, float maxHp)
    {
        AttackForce = attackForce;
        AttackRange = attackRange;
        DetectRange = detectRange;
        mMaxHP = maxHp;
    }

    protected abstract void ConfigureStats(int difficulty);

    protected virtual void Update()
    {
        transform.rotation = cameraPivot.rotation;
        TickMonster();
    }

    protected virtual void TickMonster() { }

    protected bool IsReleaseAttempt(float releaseRange)
    {
        return !isReleased && Vector3.Distance(transform.position, playerTransform.position) <= releaseRange &&
            player.playerType == PlayerType.bomb && Input.GetKeyDown(KeyCode.Q);
    }

    protected void TryRelease(float releaseRange)
    {
        if (!IsReleaseAttempt(releaseRange)) return;

        if (player.pCurrExp < player.needExp)
        {
            player.ShowInsufficientExperience();
            return;
        }

        player.pCurrExp -= player.needExp;
        ReleaseMonster();
        Debug.Log("Seal monster released.");
    }

    protected void ReleaseMonster()
    {
        isReleased = true;
        rb.isKinematic = false;
        rb.useGravity = UsesGravity;
    }

    public void SetCameraPivot(Transform pivot)
    {
        cameraPivot = pivot;
    }

    public virtual void Move(Vector3 direction, float speed)
    {
        rb.MovePosition(rb.position + direction * Time.deltaTime * speed);
    }

    protected void MoveTo(Vector3 position)
    {
        rb.MovePosition(position);
    }

    public virtual void TakeDamage(float damage)
    {
        if (monIsDead || StartsSealed && !isReleased) return;

        mCurHP -= damage;
        Debug.Log($"{name} HP: {mCurHP}/{mMaxHP}", this);
        if (mCurHP > 0) return;

        monIsDead = true;
        OnDeath();
    }

    public void Stun(float duration)
    {
        stunnedUntil = Mathf.Max(stunnedUntil, Time.time + Mathf.Max(0f, duration));
    }

    public void PullTo(Vector3 position)
    {
        if (rb != null && !rb.isKinematic) rb.MovePosition(position);
        else transform.position = position;
    }

    public void ApplyDamageOverTime(float damagePerSecond, float duration)
    {
        StartCoroutine(DamageOverTime(damagePerSecond, duration));
    }

    private IEnumerator DamageOverTime(float damagePerSecond, float duration)
    {
        float remaining = duration;
        while (!monIsDead && remaining > 0f)
        {
            float interval = Mathf.Min(1f, remaining);
            yield return new WaitForSeconds(interval);
            if (monIsDead) yield break;
            TakeDamage(damagePerSecond * interval);
            remaining -= interval;
        }
    }

    protected virtual void OnDeath() { }

    protected void SpawnDeathMarkAndDestroy(int experience, int score)
    {
        BoxCollider col = GetComponent<BoxCollider>();
        if (deathMark != null && col != null)
        {
            Instantiate(deathMark,
                new Vector3(transform.position.x, col.bounds.min.y + 0.1f, transform.position.z),
                Quaternion.Euler(90, 0, 0));
        }

        FindFirstObjectByType<MonsterSpawn>()?.AddScore(score);
        player.GetExp(experience);
        Destroy(gameObject);
    }
}
