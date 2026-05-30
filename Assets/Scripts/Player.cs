using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerType
    {
        bomb,
        boo
    }
    PlayerType playerType;

    private Rigidbody2D rb;
    [SerializeField] private Transform pAttackPoint;
    private float pSpeed = 5f;
    private float pJumpForce = 5f;
    private float pAttackForce = 10f;
    private Vector2 pAttackRange;
    private float pHP = 100f;

    [SerializeField] private Monster monster;
    LayerMask monsterLayer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerType = PlayerType.bomb;

        pAttackRange = new Vector2(1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (monster.mCurHP <= 0)
        {
            Time.timeScale = 0f;
            return;
        }
        
        KeyInput();
    }

    void KeyInput()
    {
        Vector3 movePosition = Vector3.zero;

        // bomb으로 전환했을 때 위에서 아래로 떨어지면 hp가 깎이는 기능 추가?
        if (playerType == PlayerType.bomb)
        {
            if (Input.GetKey(KeyCode.A))
            {
                movePosition = Vector3.left;
                transform.Translate(-pSpeed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                movePosition = Vector3.right;
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
                movePosition = Vector3.left;
                transform.Translate(-pSpeed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(0, -pSpeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                movePosition = Vector3.right;
                transform.Translate(pSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                rb.gravityScale = 1;
                playerType = PlayerType.bomb;
                Debug.Log("Tab, booo to bomb");
            }
        }
    }

    void AttackRange()
    {
        Debug.Log("AttackRange() 함수 실행");
        
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pAttackPoint.position, pAttackRange, monsterLayer);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Monster"))
            {
                monster.TakeDamage(pAttackForce);
            }

            Debug.Log("감지된 몬스터 수: " + colliders.Length);
            Debug.Log(collider.name);
        }
    }
}
