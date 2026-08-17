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
            LoadScene("GameBossBattleScene");
        }
        if (uiStart.modeIndex == 2)
        {
            LoadScene("GameScoreScene");
        }
    }

    public void GameToStart()
    {
        LoadScene("StartScene");
    }

    public static void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    private static void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
