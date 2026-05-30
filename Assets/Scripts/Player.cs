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
    private float pSpeed = 5f;
    private float pJumpForce = 5f;
    private float pAttackForce = 10f;
    private float pAttackRange = 2f;
    private float pHP = 100f;

    //[SerializeField] private Monster monster;
    [SerializeField] private Monster monster;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerType = PlayerType.bomb;
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
        // bomb으로 전환했을 때 위에서 아래로 떨어지면 hp가 깎이는 기능 추가?
        if (playerType == PlayerType.bomb)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(-pSpeed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
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
                monster.TakeDamage(pAttackForce);
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
                transform.Translate(-pSpeed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(0, -pSpeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
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
}
