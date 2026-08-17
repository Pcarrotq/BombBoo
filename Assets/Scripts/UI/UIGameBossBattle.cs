using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGameBossBattle : MonoBehaviour
{
    [SerializeField] private Player player;
    PlayerType playerType;

    [SerializeField] private Monster monster;
    
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider expBar;
    [SerializeField] private Slider absorptionBar;

    [SerializeField] private TMP_Text booTimer;
    [SerializeField] private GameObject booTimerPanel;
    [SerializeField] private TMP_Text insufficientExperienceText;
    [SerializeField] private TMP_FontAsset koreanFontAsset;
    private Coroutine insufficientExperienceCoroutine;
    private bool supportsKorean;

    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject gameClaerPanel;
    [SerializeField] private GameObject gameOverPanel;

    void Start()
    {
        if (insufficientExperienceText != null)
        {
            supportsKorean = ApplyKoreanFont();
            insufficientExperienceText.gameObject.SetActive(false);
        }

        hpBar.maxValue = player.pMaxHP;
        expBar.maxValue = player.pMaxExp;
        absorptionBar.maxValue = player.pAbsorptionLimit;
    }

    void Update()
    {
        hpBar.value = player.pCurrHP;
        expBar.value = player.pCurrExp;
        absorptionBar.value = player.pAbsorption;

        playerType = player.playerType;

        if (playerType == PlayerType.boo)
        {
            booTimerPanel.SetActive(true);
        }
        else
        {
            booTimerPanel.SetActive(false);
        }

        booTimer.text = $"{player.booTimer:N2}";

        GameClear();
        GameOver();
    }

    public void OnClickSkill(int skillNumber)
    {
        player.UseSkill(skillNumber);
    }

    public void ShowInsufficientExperience()
    {
        ShowWarning("경험치가 충분하지 않습니다.", "Not enough experience.");
    }

    private bool ApplyKoreanFont()
    {
        if (koreanFontAsset == null) return false;

        insufficientExperienceText.font = koreanFontAsset;
        return true;
    }

    public void ShowWarning(string message, string fallbackMessage)
    {
        if (insufficientExperienceText == null) return;

        if (insufficientExperienceCoroutine != null)
        {
            StopCoroutine(insufficientExperienceCoroutine);
        }

        insufficientExperienceCoroutine = StartCoroutine(
            ShowWarningMessage(supportsKorean ? message : fallbackMessage));
    }

    private IEnumerator ShowWarningMessage(string message)
    {
        insufficientExperienceText.text = message;
        insufficientExperienceText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        insufficientExperienceText.gameObject.SetActive(false);
        insufficientExperienceCoroutine = null;
    }

    public void SettingPanelOpen()
    {
        settingPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SettingPanelClose()
    {
        settingPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GameClear()
    {
        if (BossBattleController.IsCleared && player.pCurrHP > 0)
        {
            gameClaerPanel.SetActive(true);
            Time.timeScale = 0f;
            return;
        }

        if (!TryGetMonster()) return;

        if (monster.monsterType == MonsterType.boss && monster.mCurHP <= 0 && player.pCurrHP > 0)
        {
            gameClaerPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void GameOver()
    {
        if (!TryGetMonster()) return;

        if (player.pCurrHP <= 0 && monster.mCurHP > 0)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RetryButton()
    {
        SceneChange.ReloadCurrentScene();
    }

    private bool TryGetMonster()
    {
        if (monster == null)
        {
            // ponytail: 보스가 생성될 때까지 프레임마다 탐색한다. 스폰 수가 커지면 MonsterSpawn 이벤트로 전달한다.
            monster = FindFirstObjectByType<Monster>();
        }

        return monster != null;
    }
}
