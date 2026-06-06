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

    Player player;

    void Start()
    {
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
    }

    // TO-DO: 몬스터마다 데미지 다르게 들어가게 하기
    public void TakeDamage(float damage)
    {
        mCurHP -= damage;
        Debug.Log("Damaged! Now Monster's HP is " + mCurHP);

        if (mCurHP <= 0)
        {
            monIsDead = true;
            Time.timeScale = 0f;
            Debug.Log("Monster is dead!");

            if (monsterType == MonsterType.spider)
            {
                Destroy(gameObject);
                player.GetExp(10);
            }
        }
    }
}
