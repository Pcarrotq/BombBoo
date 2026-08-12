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

    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject gameClaerPanel;
    [SerializeField] private GameObject gameOverPanel;

    void Start()
    {
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
        if (monster.monsterType == MonsterType.boss && monster.mCurHP <= 0 && player.pCurrHP > 0)
        {
            gameClaerPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void GameOver()
    {
        if (player.pCurrHP <= 0 && monster.mCurHP > 0)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RetryButton()
    {
        player.pCurrHP = player.pMaxHP;
        monster.mCurHP = monster.mMaxHP;
        monster.monIsDead = false;
        monster.miniBossNum = monster.miniBossNumMax;
        gameClaerPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
