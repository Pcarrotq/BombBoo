using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    enum MonsterType
    {
        boss,
        miniboss
    }
    [SerializeField] private MonsterType monsterType;

    public float mCurHP;
    float mMaxHP;

    // Start is called before the first frame update
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

        mCurHP = mMaxHP;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TakeDamage(float damage)
    {
        mCurHP -= damage;
        Debug.Log("Damaged! Now Monster's HP is " + mCurHP);

        if (mCurHP <= 0)
        {
            Time.timeScale = 0f;
            Debug.Log("Monster is dead!");
        }
    }
}
