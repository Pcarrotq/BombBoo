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
    private Vector3 pAttackRange;
    private int pAttackSkillNum;
    public int[] pAttackSkillNums;
    public List<int> pAttackSkills;

    public float pMaxHP;
    public float pCurrHP;
    
    public int pMaxExp;
    public int pCurrExp;

    public int needExp;

    public int pAbsorption; // 현재 흡수한 양
    private int pAbsorptionAmount; // 흡수량
    public int pAbsorptionLimit; // 흡수 한계량
    private int pAbsorptionLow; // 흡수 내보내기

    [SerializeField] private Transform cameraPivot;

    public float booTimer;
    [SerializeField] private UIGameScore uiGameScore;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator = GetComponent<Animator>();

        playerType = PlayerType.bomb;

        pAttackForce = 10f;
        pAttackRange = new Vector3(2f, 2f, 2f);
        pAttackSkillNums = new int[3];
        pAttackSkills = new List<int>();

        for (int i = 1; i <= 10; i++)
        {
            pAttackSkills.Add(i);
        }

        pMaxHP = 100f;
        pCurrHP = pMaxHP;

        pMaxExp = 100;
        pCurrExp = 0;

        needExp = 100;

        pAbsorption = 0;
        pAbsorptionAmount = 10;
        pAbsorptionLimit = 100;
        pAbsorptionLow = 10;

        booTimer = 5f;

        if (GameManager.Instance.modeIndex == 2)
        {
            AttackSkill();
        }
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();
        FollowCameraRotate();

        if (GameManager.Instance.modeIndex == 2 && pCurrExp >= pMaxExp)
        {
            AttackSkill();
            return;
        }

        Debug.Log("curr exp = " + pCurrExp);
        Debug.Log("need exp = " + needExp);
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
                pCurrExp -= 5;
                
                // TO-DO: 아 뭐 좋은 아이디어 있었는데 까먹었다
                
                // 다른 것
                // 죽음 마크 흡수하기
                pAbsorption += pAbsorptionAmount;
                if (pAbsorption >= pAbsorptionLimit) // 죽음 마크 흡수량을 한계치보다 많이 흡수하면
                {
                    while (pCurrHP == 0) // hp가 다 깎일 때까지 플레이어에게 데미지 주기
                    {
                        TakeDamage(10);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // Q 키를 눌러 죽음 마크 흡수 낮추기
                pAbsorption -= pAbsorptionLow;
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

    void AttackSkill()
    {
        for (int i = 0; i < pAttackSkills.Count; i++)
        {
            int randomSkill = Random.Range(i, pAttackSkills.Count);

            pAttackSkillNum = pAttackSkills[i];
            pAttackSkills[i] = pAttackSkills[randomSkill];
            pAttackSkills[randomSkill] = pAttackSkillNum;
        }

        for (int i = 0; i < 3; i++)
        {
            pAttackSkillNums[i] = pAttackSkills[i];
            Debug.Log($"현재 스킬 {i + 1} : {pAttackSkillNums[i]}");
        }

        uiGameScore?.SkillButtons();
    }

    public void UseSkill(int skillNumber)
    {
        switch (skillNumber)
        {
            case 1:
                Debug.Log("스킬 1");
                break;
            case 2:
                Debug.Log("스킬 2");
                break;
            case 3:
                Debug.Log("스킬 3");
                break;
            case 4:
                Debug.Log("스킬 4");
                break;
            case 5:
                Debug.Log("스킬 5");
                break;
            case 6:
                Debug.Log("스킬 6");
                break;
            case 7:
                Debug.Log("스킬 7");
                break;
            case 8:
                Debug.Log("스킬 8");
                break;
            case 9:
                Debug.Log("스킬 9");
                break;
            case 10:
                Debug.Log("스킬 10");
                break;
        }
    }

    void DestroyDeathMark()
    {
        Collider[] colliders = Physics.OverlapBox(pAttackPoint.position, pAttackRange);

        foreach (Collider collider in colliders)
        {
            MonsterDeathMark deathMark = collider.GetComponent<MonsterDeathMark>();

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
            pCurrExp = 0;
        }
    }

    public void GetExp(int exp)
    {
        pCurrExp += exp;
        Debug.Log("Player Exp = " + pCurrExp);
    }
}
