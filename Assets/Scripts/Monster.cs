using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterType monsterType;
    MonsterState monsterState;

    private bool isReleased;
    private float releaseRange;

    private int mAttackForce;
    public float mAttackRange;

    float mDetectRange;

    public float mCurHP;
    public float mMaxHP;
    public bool monIsDead = false;
    [SerializeField] private DeathMark deathMark;

    public int mioniBossNum;
    public int mioniBossNumMax;

    private Rigidbody rb;

    Player player;
    float target;
    Transform playerTrf;

    [SerializeField] private Transform cameraPivot;

    void Start()
    {
        isReleased = false;
        releaseRange = 3f;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // TO-DO: 몬스터마다 deathmark 넓이 달라지게 하기?
        if (GameManager.Instance.diffIndex == 1)
        {
            if (monsterType == MonsterType.boss)
            {
                mAttackForce = 10;
                mAttackRange = 10f;
                mMaxHP = 1000f;
            }
            if (monsterType == MonsterType.miniboss)
            {
                mAttackForce = 5;
                mAttackRange = 5f;
                mMaxHP = 500f;
            }
            if (monsterType == MonsterType.sealMonster)
            {
                mAttackForce = 4;
                mAttackRange = 4f;
                mDetectRange = 6f;
                mMaxHP = 400f;
            }
            if (monsterType == MonsterType.spider)
            {
                mAttackForce = 1;
                mAttackRange = 1f;
                mDetectRange = 5f;
                mMaxHP = 50f;
            }
        }
        if (GameManager.Instance.diffIndex == 2)
        {
            if (monsterType == MonsterType.boss)
            {
                mAttackForce = 20;
                mAttackRange = 20f;
                mMaxHP = 2000f;
            }
            if (monsterType == MonsterType.miniboss)
            {
                mAttackForce = 10;
                mAttackRange = 10f;
                mMaxHP = 1000f;
            }
            if (monsterType == MonsterType.sealMonster)
            {
                mAttackForce = 9;
                mAttackRange = 9f;
                mDetectRange = 11f;
                mMaxHP = 500f;
            }
            if (monsterType == MonsterType.spider)
            {
                mAttackForce = 5;
                mAttackRange = 5f;
                mDetectRange = 10f;
                mMaxHP = 100f;
            }
        }
        if (GameManager.Instance.diffIndex == 3)
        {
            if (monsterType == MonsterType.boss)
            {
                mAttackForce = 30;
                mAttackRange = 30f;
                mMaxHP = 3000f;
            }
            if (monsterType == MonsterType.miniboss)
            {
                mAttackForce = 20;
                mAttackRange = 20f;
                mMaxHP = 2000f;
            }
            if (monsterType == MonsterType.sealMonster)
            {
                mAttackForce = 15;
                mAttackRange = 15f;
                mDetectRange = 25f;
                mMaxHP = 1000f;
            }
            if (monsterType == MonsterType.spider)
            {
                mAttackForce = 10;
                mAttackRange = 10f;
                mDetectRange = 20f;
                mMaxHP = 200f;
            }
        }

        mCurHP = mMaxHP;

        mioniBossNum = mioniBossNumMax;
        mioniBossNumMax = 4;

        player = FindObjectOfType<Player>();
        playerTrf = player.transform;

        // 봉인된 동안 움직이지 않도록 한다.
        if (monsterType == MonsterType.sealMonster)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        monsterState = MonsterState.Idle;
    }

    void Update()
    {
        if (player.playerType == PlayerType.bomb && Input.GetKeyDown(KeyCode.Tab))
        {
            rb.useGravity = true;
        }
        if (player.playerType == PlayerType.boo && Input.GetKeyDown(KeyCode.Tab))
        {
            rb.useGravity = false;
        }
        
        FollowCameraRotate();

        if (monsterType == MonsterType.miniboss)
        {
            if (mCurHP <= 0)
            {
                mioniBossNum -= 1;
            }
        }
        if (monsterType == MonsterType.sealMonster)
        {
            if (!isReleased)
            {
                float distance = Vector3.Distance(transform.position, playerTrf.position);

                if (distance <= releaseRange && player.playerType == PlayerType.boo && Input.GetKeyDown(KeyCode.Q))
                {
                    if (player.pCurrExp >= player.needExp)
                    {
                        player.pCurrExp -= player.needExp;

                        isReleased = true;

                        rb.isKinematic = false;
                        rb.useGravity = true;

                        monsterState = MonsterState.Idle;

                        Debug.Log("봉인이 해제되었습니다!");
                    }
                    else
                    {
                        Debug.Log("Exp가 부족합니다.");
                    }
                }
            }
        }
        if (monsterType == MonsterType.sealMonster && isReleased)
        {
            MonsterAI();
        }
        if (monsterType == MonsterType.spider)
        {
            MonsterAI();
        }
    }

    void FollowCameraRotate()
    {
        transform.rotation = cameraPivot.rotation;
    }

    // TO-DO: 몬스터마다 데미지 다르게 들어가게 하기
    public void TakeDamage(float damage)
    {
        mCurHP -= damage;
        Debug.Log("Damaged! Now Monster's HP is " + mCurHP);

        if (mCurHP <= 0)
        {
            BoxCollider col = GetComponent<BoxCollider>();
            monIsDead = true;
            Debug.Log("Monster is dead!");

            if (monsterType == MonsterType.boss)
            {
                Time.timeScale = 0f;
            }
            if (monsterType == MonsterType.spider)
            {
                Instantiate(
                    deathMark,
                    new Vector3(transform.position.x, col.bounds.min.y + 0.1f, transform.position.z),
                    Quaternion.Euler(90, 0, 0)
                ); // transform.position, deathMark.transform.rotation
                Destroy(gameObject);
                player.GetExp(100);
            }
        }
    }

    void MonsterAI()
    {
        target = Vector2.Distance(transform.position, playerTrf.position);

        switch (monsterState)
        {
            // -- Idle -------------------------------------------------
            case MonsterState.Idle:
                Debug.Log("Monster Idle");

                // 감지 범위에 들어오면
                if (target < mDetectRange)
                {
                    // 쫓기
                    monsterState = MonsterState.Chase;
                }

                break;
            
            
            // -- Chase ------------------------------------------------
            case MonsterState.Chase:
                Debug.Log("Monster Chase");

                Vector3 dir = (playerTrf.position - transform.position).normalized;
                transform.position += dir * Time.deltaTime * 2f;
                
                // 범위에서 나가면
                if (target > mDetectRange)
                {
                    // 멈추기
                    monsterState = MonsterState.Idle;
                }
                
                // 공격 범위에 들어오면
                if (target < mAttackRange)
                {
                    monsterState = MonsterState.Attack;
                }
                
                break;
            
            
            // -- Attack -----------------------------------------------
            case MonsterState.Attack:
                Debug.Log("Monster Attack");

                if (target < mAttackRange)
                {
                    player.TakeDamage(mAttackForce);
                }
                else
                {
                    monsterState = MonsterState.Chase;
                }

                break;
        }
    }

    void SealMonster()
    {
        if (monsterType == MonsterType.sealMonster)
        {
            
        }
    }
}
