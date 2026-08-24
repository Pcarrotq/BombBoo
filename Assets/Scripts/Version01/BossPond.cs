using UnityEngine;

public class BossPond : MonoBehaviour
{
    [SerializeField] private Boss01 sealedBoss;
    [SerializeField] private Color clueColor = Color.white;
    [SerializeField] private Color activatedColor = Color.yellow;
    private CameraController01 cameraController;
    private SpriteRenderer spriteRenderer;
    private Color normalColor;
    private int correctViewIndex;
    public bool IsActivated { get; private set; }
    public bool IsCurrentTarget { get; private set; }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) normalColor = spriteRenderer.color;
        if (sealedBoss == null) sealedBoss = GetComponentInParent<Boss01>();
        if (sealedBoss == null)
        {
            Debug.LogError("BossPond must be placed under Boss01.", this);
            return;
        }

        sealedBoss.RegisterPond(this);
    }

    void Update()
    {
        if (spriteRenderer == null || IsActivated) return;
        bool showClue = IsCurrentTarget && cameraController != null && !cameraController.IsRotating &&
            cameraController.SideViewIndex == correctViewIndex;
        spriteRenderer.color = showClue ? clueColor : normalColor;
    }

    public void Initialize(CameraController01 controller, int viewIndex)
    {
        cameraController = controller;
        correctViewIndex = viewIndex;
        IsActivated = false;
        IsCurrentTarget = false;
        if (spriteRenderer != null) spriteRenderer.color = normalColor;
    }

    public void SetCurrentTarget(bool isCurrent)
    {
        IsCurrentTarget = isCurrent;
        if (!isCurrent && !IsActivated && spriteRenderer != null) spriteRenderer.color = normalColor;
    }

    public void TryActivate()
    {
        if (!IsCurrentTarget || IsActivated || cameraController == null || cameraController.IsRotating ||
            cameraController.SideViewIndex != correctViewIndex) return;

        IsCurrentTarget = false;
        IsActivated = true;
        if (spriteRenderer != null) spriteRenderer.color = activatedColor;
        sealedBoss?.ActivatePond(this);
    }

    public void RestoreActivatedState()
    {
        IsCurrentTarget = false;
        IsActivated = true;
        if (spriteRenderer != null) spriteRenderer.color = activatedColor;
    }
}
