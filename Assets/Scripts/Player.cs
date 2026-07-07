using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer sprite;

    public PlayerType playerType;
    Monster monster;

    private Rigidbody rb;
    private Animator animator;
    
    private float pSpeed = 5f;
    private bool isGround;

    private float pJumpForce = 5f;

    [SerializeField] private Transform pAttackPoint;
    private float pAttackForce;
    private Vector2 pAttackRange;

    public float pMaxHP;
    public float pCurrHP;
    
    public int pMaxExp;
    public int pCurExp;
    public int pLevel;

    public float booTimer;

    [SerializeField] private Transform cameraPivot;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator = GetComponent<Animator>();

        playerType = PlayerType.bomb;

        pAttackForce = 10f;
        pAttackRange = new Vector2(1f, 1f);

        pMaxHP = 100f;
        pCurrHP = pMaxHP;

        pMaxExp = 100;
        pCurExp = 0;
        pLevel = 0;

        booTimer = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();
        FollowCameraRotate();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            animator.SetBool("IsGround", true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
            animator.SetBool("IsGround", false);
        }
    }

    void FollowCameraRotate()
    {
        transform.rotation = cameraPivot.rotation;
    }

    void KeyInput()
    {
        animator.SetBool("isMoving", false);

        // bomb으로 전환했을 때 위에서 아래로 떨어지면 hp가 깎이는 기능 추가?
        if (playerType == PlayerType.bomb)
        {
            if (Input.GetKey(KeyCode.A))
            {
                sprite.flipX = false;

                animator.SetBool("isMoving", true);
                transform.Translate(Vector3.left * pSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                sprite.flipX = true;

                animator.SetBool("isMoving", true);
                transform.Translate(Vector3.right * pSpeed * Time.deltaTime);
            }

            // 점프 준비
            // 땅에 있을 동안
            if (Input.GetKey(KeyCode.Space) && isGround)
            {
                animator.SetBool("isJumpReady", true);
            }
            // 점프하지 않는다면
            /*else
            {
                animator.SetBool("isJumpReady", false);
            }*/
            
            // 점프하는 동안
            if (Input.GetKeyUp(KeyCode.Space) && isGround)
            {
                animator.SetBool("isJumpReady", false);
                animator.SetBool("IsJump", true);

                rb.AddForce(Vector3.up * pJumpForce, ForceMode.Impulse);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                rb.useGravity = false; // boo일 때는 중력에 영향을 받지 않는다.
                playerType = PlayerType.boo;
                animator.SetBool("isBoo", true);
                Debug.Log("Tab, bomb to boo");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                AttackRange();
            }
        }
        else if (playerType == PlayerType.boo)
        {
            if (booTimer > 0)
            {
                booTimer -= Time.deltaTime;
            }
            else if (booTimer <= 0f)
            {
                rb.useGravity = true; // bomb일 때는 중력에 영향을 받는다.
                playerType = PlayerType.bomb;
                animator.SetBool("isBoo", false);
                booTimer = 5f;
                return;
            }

            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("isMoving", true);
                transform.Translate(Vector3.up * pSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                sprite.flipX = false;
                animator.SetBool("isMoving", true);
                transform.Translate(Vector3.left * pSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool("isMoving", true);
                transform.Translate(Vector3.down * pSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                sprite.flipX = true;
                animator.SetBool("isMoving", true);
                transform.Translate(Vector3.right * pSpeed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                rb.useGravity = true; // bomb일 때는 중력에 영향을 받는다.
                playerType = PlayerType.bomb;
                animator.SetBool("isBoo", false);
                booTimer = 5f;
                Debug.Log("Tab, booo to bomb");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                // TO-DO: 죽음 마크 사라지기
                DestroyDeathMark();
                // TO-DO: 점수 깎게 만들기
                pCurExp -= 5;
                // TO-DO: 아 뭐 좋은 아이디어 있었는데 까먹었다
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

    void DestroyDeathMark()
    {
        Collider[] colliders = Physics.OverlapBox(pAttackPoint.position, pAttackRange);

        foreach (Collider collider in colliders)
        {
            DeathMark deathMark = collider.GetComponent<DeathMark>();

            if (collider.CompareTag("DeathMark"))
            {
                deathMark.DestroyMark();
            }

            /*if (monster != null)
            {
                monster.TakeDamage(pAttackForce);
            }*/
        }
    }

    public void TakeDamage(float damage)
    {
        if (playerType == PlayerType.boo) return;
        pCurrHP -= damage;

        Debug.Log("Player " + damage + "Damage!");

        // TO-DO: 보스를 처치함과 동시에 죽었는가?
        if (pCurrHP <= 0)
        {
            Time.timeScale = 0f;
            pCurExp = 0;
        }
    }

    public void GetExp(int exp)
    {
        pCurExp += exp;
        Debug.Log("Player Exp = " + pCurExp);
    }

    public void SetLevel()
    {
        if (pCurExp == 100)
        {
            pCurExp = 0;
            pLevel += 1;
        }
    }
}
