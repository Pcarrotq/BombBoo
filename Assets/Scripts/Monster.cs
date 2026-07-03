using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private MonsterType monsterType;
    MonsterState monsterState;

    private int mAttackForce;
    public float mAttackRange;

    float mDetectRange;

    public float mCurHP;
    public float mMaxHP;
    public bool monIsDead = false;
    [SerializeField] private DeathMark deathMark;

    private Rigidbody rb;

    Player player;
    PlayerType playerType;
    float target;
    Transform playerTrf;

    [SerializeField] private Transform cameraPivot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // TO-DO: 몬스터마다 deathmark 넓이 달라지게 하기?
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
        if (monsterType == MonsterType.spider)
        {
            mAttackForce = 1;
            mAttackRange = 1f;
            mDetectRange = 5f;
            mMaxHP = 50f;
        }

        mCurHP = mMaxHP;

        player = FindObjectOfType<Player>();
        playerTrf = player.transform;
        monsterState = MonsterState.Idle;
    }

    void Update()
    {
        if (playerType == PlayerType.bomb && Input.GetKeyDown(KeyCode.Tab))
        {
            rb.useGravity = true;
        }
        if (playerType == PlayerType.boo && Input.GetKeyDown(KeyCode.Tab))
        {
            rb.useGravity = false;
        }
        
        FollowCameraRotate();

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
                player.GetExp(10);
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
}
