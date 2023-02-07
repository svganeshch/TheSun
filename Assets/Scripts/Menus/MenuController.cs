using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject pausePanel;

    private MyInputActions inputActions;
    private InputAction pauseAction;

    private void Awake()
    {
        inputActions = new MyInputActions();
    }

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void OnPauseAction(InputAction.CallbackContext ctx)
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
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

    private void OnEnable()
    {
        pauseAction = inputActions.UI.Pause;
        pauseAction.Enable();
        pauseAction.performed += OnPauseAction;
    }

    private void OnDisable()
    {
        pauseAction.Disable();
    }
}
