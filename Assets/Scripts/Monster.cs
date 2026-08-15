using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterType monsterType;

    private bool isReleased;
    private float releaseRange;
    private int mAttackForce;
    private float mAttackRange;
    private float mDetectRange;

    public float mCurHP;
    public float mMaxHP;
    public bool monIsDead;
    [SerializeField] private MonsterDeathMark deathMark;

    public int miniBossNum;
    public int miniBossNumMax;
    private bool isminibossCounted;

    private Rigidbody rb;
    private MonsterAI monsterAI;
    private Player player;
    private Transform playerTrf;
    [SerializeField] private Transform cameraPivot;

    private void Start()
    {
        isReleased = false;
        releaseRange = 3f;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = true;

        SetStats(GameManager.Instance != null ? Mathf.Clamp(GameManager.Instance.diffIndex, 1, 3) : 1);
        mCurHP = mMaxHP;

        miniBossNumMax = 4;
        miniBossNum = miniBossNumMax;

        player = FindFirstObjectByType<Player>();
        if (player == null || cameraPivot == null)
        {
            Debug.LogError("Player or Camera Pivot is not assigned.", this);
            enabled = false;
            return;
        }

        playerTrf = player.transform;

        if (monsterType == MonsterType.sealMonster)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        monsterAI = GetComponent<MonsterAI>();
        if (monsterAI == null)
        {
            monsterAI = gameObject.AddComponent<MonsterAI>();
        }

        monsterAI.Initialize(this, rb, player, mDetectRange, mAttackRange, mAttackForce);
    }

    private void SetStats(int difficulty)
    {
        if (monsterType == MonsterType.boss)
        {
            mAttackForce = 10 * difficulty;
            mAttackRange = 10f * difficulty;
            mMaxHP = 1000f * difficulty;
        }
        else if (monsterType == MonsterType.miniboss)
        {
            mAttackForce = 5 * difficulty;
            mAttackRange = 5f * difficulty;
            mMaxHP = 500f * difficulty;
        }
        else if (monsterType == MonsterType.sealMonster)
        {
            mAttackForce = difficulty == 1 ? 4 : difficulty == 2 ? 9 : 15;
            mAttackRange = difficulty == 1 ? 4f : difficulty == 2 ? 9f : 15f;
            mDetectRange = difficulty == 1 ? 6f : difficulty == 2 ? 11f : 25f;
            mMaxHP = difficulty == 1 ? 400f : difficulty == 2 ? 500f : 1000f;
        }
        else if (monsterType == MonsterType.spider)
        {
            mAttackForce = difficulty == 1 ? 1 : difficulty == 2 ? 5 : 10;
            mAttackRange = difficulty == 1 ? 1f : difficulty == 2 ? 5f : 10f;
            mDetectRange = 5f * difficulty;
            mMaxHP = 50f * difficulty;
        }
    }

    private void Update()
    {
        FollowCameraRotate();

        if (monsterType == MonsterType.miniboss && mCurHP <= 0 && !isminibossCounted)
        {
            miniBossNum -= 1;
            isminibossCounted = true;
        }

        if (monsterType != MonsterType.sealMonster || isReleased) return;

        float distance = Vector3.Distance(transform.position, playerTrf.position);
        if (distance > releaseRange || player.playerType != PlayerType.boo || !Input.GetKeyDown(KeyCode.Q)) return;

        if (player.pCurrExp < player.needExp)
        {
            Debug.Log("Not enough experience.");
            return;
        }

        player.pCurrExp -= player.needExp;
        isReleased = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        Debug.Log("Seal monster released.");
    }

    public bool CanRunAI => !monIsDead &&
        (monsterType == MonsterType.spider || (monsterType == MonsterType.sealMonster && isReleased));

    public void SetCameraPivot(Transform pivot)
    {
        cameraPivot = pivot;
    }

    private void FollowCameraRotate()
    {
        transform.rotation = cameraPivot.rotation;
    }

    public void TakeDamage(float damage)
    {
        if (monIsDead) return;

        mCurHP -= damage;
        Debug.Log("Damaged! Now Monster's HP is " + mCurHP);

        if (mCurHP > 0) return;

        BoxCollider col = GetComponent<BoxCollider>();
        monIsDead = true;
        Debug.Log("Monster is dead!");

        if (monsterType == MonsterType.boss)
        {
            Time.timeScale = 0f;
        }
        else if (monsterType == MonsterType.spider)
        {
            Instantiate(
                deathMark,
                new Vector3(transform.position.x, col.bounds.min.y + 0.1f, transform.position.z),
                Quaternion.Euler(90, 0, 0));
            Destroy(gameObject);
            player.GetExp(100);
        }
    }
}
