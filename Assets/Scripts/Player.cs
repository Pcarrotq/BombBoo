using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PlayerType
{
    bomb,
    boo
}

public class Player : MonoBehaviour
{
    PlayerType playerType;
    Monster monster;

    private Rigidbody rb;
    
    private float pSpeed = 5f;
    private float pJumpForce = 5f;

    [SerializeField] private Transform pAttackPoint;
    private float pAttackForce = 10f;
    private Vector2 pAttackRange;

    public float pCurrHP;
    public float pMaxHP;
    
    int pLevel = 0;
    [SerializeField] private TMP_Text pLevelText;

    [SerializeField] private Transform cameraPivot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        playerType = PlayerType.bomb;

        pAttackRange = new Vector2(1f, 1f);

        pMaxHP = 100f;
        pCurrHP = pMaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        pLevelText.text = $"{pLevel}";
        KeyInput();
        FollowCameraRotate();
    }

    void FollowCameraRotate()
    {
        transform.rotation = cameraPivot.rotation;
    }

    void KeyInput()
    {
        // bomb으로 전환했을 때 위에서 아래로 떨어지면 hp가 깎이는 기능 추가?
        if (playerType == PlayerType.bomb)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * pSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * pSpeed * Time.deltaTime);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * pJumpForce, ForceMode.Impulse);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                rb.useGravity = true;
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
                transform.Translate(Vector3.forward * pSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.right * pSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * pSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * pSpeed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                rb.useGravity = false;
                playerType = PlayerType.bomb;
                Debug.Log("Tab, booo to bomb");
            }
        }
    }

    void AttackRange()
    {
        Collider[] colliders = Physics.OverlapBox(pAttackPoint.position, pAttackRange);

        foreach (Collider collider in colliders)
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

        // TO-DO: 보스를 처치함과 동시에 죽었는가?
        if (pCurrHP <= 0)
        {
            Time.timeScale = 0f;
            pLevel = 0;
        }
    }

    public void GetExp(int exp)
    {
        pLevel += exp;
        Debug.Log("Player Exp = " + pLevel);
    }
}
