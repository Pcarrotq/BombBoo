using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    bomb,
    boo
}

public class Player : MonoBehaviour
{
    PlayerType playerType;
    MonsterType monsterType;
    Monster monster;

    private Rigidbody2D rb;
    [SerializeField] private Transform pAttackPoint;
    private float pSpeed = 5f;
    private float pJumpForce = 5f;
    private float pAttackForce = 10f;
    private Vector2 pAttackRange;
    private int facingDir = 1;

    public float pCurrHP;
    public float pMaxHP;
    
    int pLevel = 0;

    [SerializeField] private LayerMask monsterLayer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerType = PlayerType.bomb;

        pAttackRange = new Vector2(1f, 1f);

        pMaxHP = 100f;
        pCurrHP = pMaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();
    }

    void KeyInput()
    {
        // bomb으로 전환했을 때 위에서 아래로 떨어지면 hp가 깎이는 기능 추가?
        if (playerType == PlayerType.bomb)
        {
            if (Input.GetKey(KeyCode.A))
            {
                facingDir = -1;
                transform.Translate(-pSpeed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                facingDir = 1;
                transform.Translate(pSpeed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                rb.AddForce(Vector2.up * pJumpForce, ForceMode2D.Impulse);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                rb.gravityScale = 0;
                playerType = PlayerType.boo;
                Debug.Log("Tab, bomb to boo");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                AttackRange();
            }
        }
        else if (playerType == PlayerType.boo)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(0, pSpeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                facingDir = -1;
                transform.Translate(-pSpeed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(0, -pSpeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                facingDir = 1;
                transform.Translate(pSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                rb.gravityScale = 1;
                playerType = PlayerType.bomb;
                Debug.Log("Tab, booo to bomb");
            }
        }
        
        pAttackPoint.localPosition = new Vector3(facingDir * 1f, 0f, 0f);
    }

    void AttackRange()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pAttackPoint.position, pAttackRange, 0, monsterLayer);

        foreach (Collider2D collider in colliders)
        {
            monster = collider.GetComponent<Monster>();

            if (collider.CompareTag("Monster"))
            {
                monster.TakeDamage(pAttackForce);
            }

            /*if (monster != null)
            {
                monster.TakeDamage(pAttackForce);
            }*/
        }
    }

    public void TakeDamage(int damage)
    {
        if (playerType == PlayerType.boo) return;
        pCurrHP -= damage;

        if (pCurrHP <= 0)
        {
            Time.timeScale = 0f;
        }
    }

    public void GetExp(int exp)
    {
        pLevel += exp;
        Debug.Log("Player Exp = " + pLevel);
    }
}
