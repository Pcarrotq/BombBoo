using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerSkill))]
public class Player : MonoBehaviour
{
    private SpriteRenderer sprite;
    public PlayerType playerType;
    private Rigidbody rb;
    private Animator animator;
    private float pSpeed = 5f;
    private bool isGround;
    private readonly HashSet<Collider> groundContacts = new HashSet<Collider>();
    private float pJumpForce = 5f;

    [SerializeField] private PlayerSkill playerSkill;
    public PlayerSkill Skill => playerSkill;

    public float pMaxHP;
    public float pCurrHP;
    public int pMaxExp;
    public int pCurrExp;
    public int needExp;
    public int pAbsorption;
    private int pAbsorptionAmount;
    public int pAbsorptionLimit;
    private int pAbsorptionLow;

    [SerializeField] private Transform cameraPivot;
    public float booTimer;
    private UIGameScore uiGameScore;
    private UIGameBossBattle uiGameBossBattle;

    void Awake()
    {
        playerSkill ??= GetComponent<PlayerSkill>();
    }

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator = GetComponent<Animator>();

        playerType = PlayerType.bomb;
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

        int modeIndex = GameManager.Instance != null ? GameManager.Instance.modeIndex : 0;
        if (modeIndex == 1)
        {
            uiGameBossBattle = FindFirstObjectByType<UIGameBossBattle>();
        }
        else if (modeIndex == 2)
        {
            uiGameScore = FindFirstObjectByType<UIGameScore>();
            RefreshSkills();
        }
    }

    void Update()
    {
        KeyInput();
        FollowCameraRotate();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;

        groundContacts.Add(collision.collider);
        UpdateGroundedState();
        animator.SetBool("IsJump", false);
    }

    void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;

        groundContacts.Remove(collision.collider);
        UpdateGroundedState();
    }

    private void UpdateGroundedState()
    {
        isGround = groundContacts.Count > 0;
        animator.SetBool("IsGround", isGround);
    }

    void FollowCameraRotate()
    {
        transform.rotation = cameraPivot.rotation;
    }

    void KeyInput()
    {
        animator.SetBool("isMoving", false);

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
            if (Input.GetKey(KeyCode.Space) && isGround)
            {
                animator.SetBool("isJumpReady", true);
            }
            if (Input.GetKeyUp(KeyCode.Space) && isGround)
            {
                animator.SetBool("isJumpReady", false);
                animator.SetBool("IsJump", true);
                rb.AddForce(Vector3.up * pJumpForce, ForceMode.Impulse);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SetPlayerType(PlayerType.boo);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerSkill.AttackRange();
            }
        }
        else if (playerType == PlayerType.boo)
        {
            if (booTimer > 0)
            {
                booTimer -= Time.deltaTime;
            }
            else
            {
                SetPlayerType(PlayerType.bomb);
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
                SetPlayerType(PlayerType.bomb);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerSkill.DestroyDeathMark();
                pCurrExp -= 5;
                pAbsorption += pAbsorptionAmount;
                if (pAbsorption >= pAbsorptionLimit)
                {
                    while (pCurrHP > 0)
                    {
                        TakeDamage(10);
                    }
                    pAbsorption = 0;
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                pAbsorption = Mathf.Max(0, pAbsorption - pAbsorptionLow);
            }
        }
    }

    private void SetPlayerType(PlayerType type)
    {
        bool isBoo = type == PlayerType.boo;
        playerType = type;
        rb.useGravity = !isBoo;
        animator.SetBool("isBoo", isBoo);
        animator.SetBool("isJumpReady", false);

        if (!isBoo)
        {
            booTimer = 5f;
        }
    }

    public void TakeDamage(float damage)
    {
        pCurrHP -= damage;
        Debug.Log("Player " + damage + "Damage!");

        if (pCurrHP <= 0)
        {
            Time.timeScale = 0f;
            pCurrExp = 0;
        }
    }

    public void GetExp(int exp)
    {
        pCurrExp += exp;
        if (uiGameScore != null)
        {
            RefreshSkills();
        }

        Debug.Log("Player Exp = " + pCurrExp);
    }

    public void ShowInsufficientExperience()
    {
        ShowWarning("경험치가 충분하지 않습니다.", "Not enough experience.");
    }

    public void ShowWarning(string message, string fallbackMessage)
    {
        if (uiGameBossBattle == null)
        {
            uiGameBossBattle = FindFirstObjectByType<UIGameBossBattle>();
        }

        uiGameBossBattle?.ShowWarning(message, fallbackMessage);
    }

    public void UseSkill(int skillNumber)
    {
        playerSkill.UseSkill(skillNumber);

        if (uiGameScore != null)
        {
            uiGameScore.SkillButtons();
        }
    }

    void RefreshSkills()
    {
        playerSkill.AttackSkill();
        uiGameScore?.SkillButtons();
    }
}
