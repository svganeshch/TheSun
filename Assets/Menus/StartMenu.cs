using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject pausePanel;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
        }
    }
    public void LoadLevelScene()
    {
        SceneManager.LoadScene("Level_1");
        Debug.Log("Loading game scene now");
    }

    public void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void Resume()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        LoadLevelScene();
    }
}
