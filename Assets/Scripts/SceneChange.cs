using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void StartToGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GameToStart()
    {
        SceneManager.LoadScene("StartScene");
    }
}
