using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalUIManager : MonoBehaviour
{
    public GameObject GameUI;
    public GameObject PauseUI;

    void Start()
    {
        GameManager.OnNewState += HandleNewState;

        GameUI.SetActive(false);
        PauseUI.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && GameManager.CurrentState != GameState.Menu)
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        bool isPaused = GameManager.CurrentState == GameState.Paused;

        if (isPaused)
        {
            GameManager.Instance.ChangeState(GameState.Playing);
        } 
        else
        {
            GameManager.Instance.ChangeState(GameState.Paused);
        }
    }

    private void HandleNewState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                GameUI.SetActive(false);
                PauseUI.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.Playing:
                GameUI.SetActive(true);
                PauseUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case GameState.Paused:
                GameUI.SetActive(true);
                PauseUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }
}
