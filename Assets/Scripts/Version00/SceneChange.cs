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
    
    public void SettingToGame()
    {
        if (GameManager.Instance == null || uiStart == null)
        {
            Debug.LogError("GameManager or UIStart is not assigned.", this);
            return;
        }

        if (uiStart.modeIndex != 1 && uiStart.modeIndex != 2)
        {
            Debug.LogWarning("Select a game mode before starting.", this);
            return;
        }

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

    public void GameToSetting()
    {
        LoadScene("SettingScene");
    }

    public void StartToGame()
    {
        LoadScene("GameScene");
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
        SceneManager.sceneLoaded -= ResumeTime;
        SceneManager.sceneLoaded += ResumeTime;
        SceneManager.LoadScene(sceneName);
    }

    private static void ResumeTime(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        SceneManager.sceneLoaded -= ResumeTime;
    }
}
