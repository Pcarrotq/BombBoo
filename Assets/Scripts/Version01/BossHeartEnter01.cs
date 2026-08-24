using UnityEngine;
using UnityEngine.SceneManagement;

public class BossHeartEnter01 : MonoBehaviour
{
    [SerializeField] private Color unlockedColor = Color.yellow;
    private SpriteRenderer spriteRenderer;
    private Color lockedColor;
    private bool isUnlocked;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) lockedColor = spriteRenderer.color;
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        if (spriteRenderer != null) spriteRenderer.color = unlocked ? unlockedColor : lockedColor;
    }

    public void TryEnter()
    {
        if (!isUnlocked || BossHeartThorn01.IsComplete) return;

        BossHeartThorn01.BeginChallenge();
        SceneManager.LoadScene("BossInsideScene");
    }
}
