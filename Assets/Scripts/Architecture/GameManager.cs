using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, Paused }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static GameState CurrentState { get; private set; } = GameState.Menu;

    public static event Action<GameState> OnNewState;

    // fields
    public string MenuScene, LevelScene;

    private bool _loadingScene;

    public SceneTransition Transitioner;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == LevelScene)
        {
            ChangeState(GameState.Playing);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ChangeState(GameState newState)
    {
        Debug.Log($"[GameManager]: Switching to game state {newState}");
        
        ExitState(CurrentState);
        CurrentState = newState;
        EnterState(CurrentState);

        SpectrumManager.Instance.ForceColorUpdate();
        OnNewState?.Invoke(CurrentState);
    }

    private void ExitState(GameState state) {
        switch (state)
        {
            case GameState.Menu:
                break;
            case GameState.Playing:
                break;
            case GameState.Paused:
                Time.timeScale = 1;
                break;
        }
    }

    private void EnterState(GameState state) {
        switch (state)
        {
            case GameState.Menu:
                break;
            case GameState.Playing:
                break;
            case GameState.Paused:
                Time.timeScale = 0;
                break;
        }
    }

    public void LoadLevel()
    {
        ChangeScene(LevelScene);
        ChangeState(GameState.Playing);
    }

    public void ChangeScene(string scene)
    {
        if (Transitioner != null && Transitioner.isActiveAndEnabled)
        {
            Transitioner.LoadScene(scene);
        } 
        else
        {
            SceneManager.LoadScene(scene);
        }
    }

    public void StartGame()
    {
        ChangeState(GameState.Playing);
        ChangeScene(LevelScene);
    }

    public void ReturnToMenu()
    {
        ChangeState(GameState.Menu);
        ChangeScene(MenuScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        SignalBus.Invoke(SaveSignal.SaveGame);
        Transitioner.QuitOut();
    }

}