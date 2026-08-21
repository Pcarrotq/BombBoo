using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGameScore : MonoBehaviour
{
    [SerializeField] private Player player;
    PlayerType playerType;

    private Monster monster;
    private MonsterSpawn monsterSpawn;
    
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider expBar;
    [SerializeField] private TMP_Text pLevelText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Slider absorptionBar;

    [SerializeField] private Button[] skillButtons;

    [SerializeField] private TMP_Text booTimer;
    [SerializeField] private GameObject booTimerPanel;

    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject gameClearPanel;
    [SerializeField] private GameObject gameOverPanel;

    void Start()
    {
        monsterSpawn = FindFirstObjectByType<MonsterSpawn>();
        hpBar.maxValue = player.pMaxHP;
        expBar.maxValue = player.pMaxExp;
        absorptionBar.maxValue = player.pAbsorptionLimit;
        
        SkillButtons();
    }

    void Update()
    {
        monster = FindFirstObjectByType<Monster>();
        
        hpBar.value = player.pCurrHP;
        expBar.value = player.pCurrExp;
        absorptionBar.value = player.pAbsorption;

        if (monsterSpawn == null)
        {
            monsterSpawn = FindFirstObjectByType<MonsterSpawn>();
        }

        if (scoreText != null && monsterSpawn != null)
        {
            scoreText.text = $"Score: {monsterSpawn.TotalScore}";
        }

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

        //GameClear();
        GameOver();
    }

    public void SkillButtons()
    {
        Debug.Log("SkillButtons");

        PlayerSkill playerSkill = player != null ? player.Skill : null;
        if (playerSkill == null || playerSkill.AttackSkillNumbers == null) return;

        // 모든 버튼 비활성화
        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i].interactable = false;
            skillButtons[i].onClick.RemoveAllListeners();
        }
        
        // 선택된 스킬은 상단 1~3번 슬롯 버튼에 순서대로 표시한다.
        for (int i = 0; i < playerSkill.AttackSkillNumbers.Length && i < skillButtons.Length; i++)
        {
            int slotIndex = i;
            skillButtons[slotIndex].interactable = true;
            skillButtons[slotIndex].onClick.AddListener(() =>
                player.UseSkillAtSlot(slotIndex));
        }
    }

    /*public void OnClickSkill(int skillNumber)
    {
        player.UseSkillAtSlot(skillNumber);
    }*/

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

    /*public void GameClear()
    {
        if (monster.monsterType == MonsterType.boss && monster.mCurHP <= 0 && player.pCurrHP > 0)
        {
            gameClearPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }*/

    public void GameOver()
    {
        if (player.pCurrHP <= 0)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RetryButton()
    {
        SceneChange.ReloadCurrentScene();
    }
}
