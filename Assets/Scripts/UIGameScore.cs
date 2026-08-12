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
    
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider expBar;
    [SerializeField] private TMP_Text pLevelText;
    [SerializeField] private Slider absorptionBar;

    [SerializeField] private Button[] skillButtons;

    [SerializeField] private TMP_Text booTimer;
    [SerializeField] private GameObject booTimerPanel;

    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject gameClearPanel;
    [SerializeField] private GameObject gameOverPanel;

    void Start()
    {
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

        if (player == null || player.pAttackSkillNums == null) return;

        // 모든 버튼 비활성화
        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i].interactable = false;
            skillButtons[i].onClick.RemoveAllListeners();
        }
        
        // 현재 가진 스킬만 활성
        foreach (int skill in player.pAttackSkillNums)
        {
            int skillBtn = skill - 1;

            if (skillBtn < 0 || skillBtn >= skillButtons.Length) continue;

            skillButtons[skillBtn].interactable = true;

            int skillNum = skill;

            skillButtons[skillBtn].onClick.AddListener(() =>
                player.UseSkill(skillNum));
        }
    }

    /*public void OnClickSkill(int skillNumber)
    {
        player.UseSkill(player.pAttackSkillNums[skillNumber]);
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
        player.pCurrHP = player.pMaxHP;
        if (monster != null)
        {
            monster.mCurHP = monster.mMaxHP;
            monster.monIsDead = false;
            monster.miniBossNum = monster.miniBossNumMax;
        }
        gameClearPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
