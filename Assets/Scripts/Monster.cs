using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    boss,
    miniboss,
    spider
}

public class Monster : MonoBehaviour
{
    [SerializeField] private MonsterType monsterType;

    public float mCurHP;
    public float mMaxHP;
    public bool monIsDead = false;
    [SerializeField] private DeathMark deathMark;

    private Rigidbody rb;

    Player player;
    PlayerType playerType;

    [SerializeField] private Transform cameraPivot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // TO-DO: 몬스터마다 deathmark 넓이 달라지게 하기?
        if (monsterType == MonsterType.boss)
        {
            mMaxHP = 1000f;
        }
        if (monsterType == MonsterType.miniboss)
        {
            mMaxHP = 500f;
        }
        if (monsterType == MonsterType.spider)
        {
            mMaxHP = 50f;
        }

        mCurHP = mMaxHP;

        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        FollowCameraRotate();

        if (playerType == PlayerType.bomb && Input.GetKeyDown(KeyCode.Tab))
        {
            rb.useGravity = true;
        }
        if (playerType == PlayerType.boo && Input.GetKeyDown(KeyCode.Tab))
        {
            rb.useGravity = false;
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
}
