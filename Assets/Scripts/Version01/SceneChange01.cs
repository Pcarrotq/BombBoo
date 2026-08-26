using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange01 : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartToGame()
    {
        Exit.ResetWaves();
        BossHeartThorn01.ResetProgress();
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
