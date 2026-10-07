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
        SceneManager.LoadScene(LevelScene);
        ChangeState(GameState.Playing);
    }

    public void StartGame()
    {
        ChangeState(GameState.Playing);
        SceneManager.LoadScene(LevelScene);
    }

    public void ReturnToMenu()
    {
        ChangeState(GameState.Menu);
        SceneManager.LoadScene(MenuScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        SignalBus.Invoke(SaveSignal.SaveGame);
        Application.Quit();
    }

}