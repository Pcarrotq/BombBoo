using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerSkill))]
public class Player : MonoBehaviour
{
    private const float DamageCooldown = 0.5f;

    private SpriteRenderer sprite;
    public PlayerType playerType;
    private Rigidbody rb;
    private Animator animator;
    private float pSpeed = 5f;
    public float MoveSpeed => pSpeed;
    private bool isGround;
    private readonly HashSet<Collider> groundContacts = new HashSet<Collider>();
    private float pJumpForce = 5f;
    public float JumpForce => pJumpForce;
    private Vector3 moveDirection;
    private float nextDamageTime;

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

        uiGameBossBattle = FindFirstObjectByType<UIGameBossBattle>();
        uiGameScore = FindFirstObjectByType<UIGameScore>();
        RefreshSkills();
    }

    void Update()
    {
        moveDirection = Vector3.zero;
        KeyInput();
        FollowCameraRotate();
    }

    void FixedUpdate()
    {
        if (moveDirection != Vector3.zero)
        {
            rb.MovePosition(rb.position + moveDirection.normalized * pSpeed * Time.fixedDeltaTime);
        }
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
            Vector3 cameraRight = Vector3.ProjectOnPlane(cameraPivot.right, Vector3.up).normalized;

            if (Input.GetKey(KeyCode.A))
            {
                sprite.flipX = false;
                animator.SetBool("isMoving", true);
                moveDirection -= cameraRight;
            }
            if (Input.GetKey(KeyCode.D))
            {
                sprite.flipX = true;
                animator.SetBool("isMoving", true);
                moveDirection += cameraRight;
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
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UseSkillAtSlot(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UseSkillAtSlot(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UseSkillAtSlot(2);
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
                moveDirection += cameraPivot.up;
            }
            if (Input.GetKey(KeyCode.A))
            {
                sprite.flipX = false;
                animator.SetBool("isMoving", true);
                moveDirection -= cameraPivot.right;
            }
            if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool("isMoving", true);
                moveDirection -= cameraPivot.up;
            }
            if (Input.GetKey(KeyCode.D))
            {
                sprite.flipX = true;
                animator.SetBool("isMoving", true);
                moveDirection += cameraPivot.right;
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SetPlayerType(PlayerType.bomb);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (playerSkill.DestroyDeathMark())
                {
                    pCurrExp = Mathf.Max(0, pCurrExp - 5);
                    pAbsorption += pAbsorptionAmount;
                    if (pAbsorption >= pAbsorptionLimit)
                    {
                        playerSkill.SelfDestruct();
                        pAbsorption = 0;
                    }
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

    public void TakeDamage(float damage, bool ignoreCooldown = false)
    {
        if (pCurrHP <= 0 || (!ignoreCooldown && Time.time < nextDamageTime)) return;

        nextDamageTime = Time.time + DamageCooldown;
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

    public void UseSkillAtSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= playerSkill.AttackSkillNumbers.Length) return;

        UseSkill(playerSkill.AttackSkillNumbers[slotIndex]);
    }

    void RefreshSkills()
    {
        playerSkill.AttackSkill();
        uiGameScore?.SkillButtons();
    }
}
