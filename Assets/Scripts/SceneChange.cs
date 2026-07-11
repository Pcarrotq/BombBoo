using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private UIStart uiStart;

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void StartToGame()
    {
        GameManager.Instance.diffIndex = uiStart.diffIndex;
        GameManager.Instance.modeIndex = uiStart.modeIndex;

        if (uiStart.modeIndex == 1)
        {
            SceneManager.LoadScene("GameBossBattleScene");
        }
        if (uiStart.modeIndex == 2)
        {
            SceneManager.LoadScene("GameScoreScene");
        }
    }

    public void GameToStart()
    {
        SceneManager.LoadScene("StartScene");
    }
}
