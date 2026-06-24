using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using UnityEngine.UI;
using TMPro;

public class UIGame : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Monster monster;
    
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider expBar;
    [SerializeField] private TMP_Text pLevelText;

    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject gameClaerPanel;
    [SerializeField] private GameObject gameOverPanel;

    void Start()
    {
        hpBar.maxValue = player.pMaxHP;
        expBar.maxValue = player.pMaxExp;
    }

    void Update()
    {
        hpBar.value = player.pCurrHP;

        expBar.value = player.pCurExp;
        pLevelText.text = $"{player.pLevel}";

        GameClear();
        GameOver();
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
        if (monster.mCurHP <= 0 && player.pCurrHP > 0)
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
        gameClaerPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
