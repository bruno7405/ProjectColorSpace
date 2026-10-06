using UnityEngine;

public class GameManager : MonoBehaviour
{
    
}


// using System;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public enum GameState { Menu, Playing, Replaying, Paused, Failed, Won, Finished, Cutscene }
// public enum ResultCondition { Death, OutOfFrame, Win }

// public class GameManager : MonoBehaviour
// {
//     public static GameManager Instance;

//     public GameState CurrentState { get; private set; } = GameState.Menu;

//     public static event Action<GameState> OnNewState;
//     public static event Action OnGameQuit;

//     public ResultCondition LastResult;

//     private bool _loadingScene;
    
//     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//     public static void Autoload()
//     {
//         GameObject autoObj = new GameObject("Game Manager");
//         autoObj.AddComponent<GameManager>();

//         Instance = autoObj.GetComponent<GameManager>();
//         DontDestroyOnLoad(autoObj);

//         if (SceneManager.GetActiveScene().name == "Level")
//         {
//             Instance.CurrentState = GameState.Playing;
//         }
//     }

//     public void ChangeState(GameState newState)
//     {
//         Debug.Log($"[GameManager]: Switching to game state {newState}");
//         UpdateState(newState);
//         CurrentState = newState;
//         OnNewState?.Invoke(newState);
//     }

//     private void UpdateState(GameState state) {
//         switch (state)
//         {
//             case GameState.Menu:
//                 break;
//             case GameState.Playing:
//                 break;
//             case GameState.Replaying:
//                 break;
//             case GameState.Failed:
//                 break;
//             case GameState.Won:
//                 break;
//             case GameState.Paused:
//                 break;
//         }
//     }

//     public void StartCutscene()
//     {
//         ChangeState(GameState.Cutscene);
//         FadeLoadScene("Cutscene");
//     }

//     public void StartGame()
//     {
//         ChangeState(GameState.Playing);
//         ClapLoadScene("Level");
//     }

//     public void Pause()
//     {
//         ChangeState(GameState.Paused);
//     }

//     public void LoseGame(ResultCondition deathReason)
//     {
//         LastResult = deathReason;
//         ChangeState(GameState.Failed);
//         PlayReplay();
//     }

//     public void ReturnToMenu()
//     {
//         GameObject music = GameObject.Find("LevelMusic");
//         if (music != null) GetComponent<LevelMusic>().PlayLevelAudio();
//         FadeLoadScene("Menu");
//         ChangeState(GameState.Menu);
//     }

//     public void WinGame()
//     {
//         LastResult = ResultCondition.Win;
//         ChangeState(GameState.Won);
//         PlayReplay(); // CHANGE THIS LATER
//     }

//     public void PlayReplay()
//     {
//         GameObject.Find("LevelMusic").GetComponent<LevelMusic>().PlayLevelAudio();
//         ChangeState(GameState.Replaying);
//         ClapLoadScene("Level");
//     }

//     public void FinishReplay()
//     {
//         ChangeState(GameState.Finished);
//         ClapLoadScene("Results");
//     }

//     public void ClapLoadScene(string sceneToLoad)
//     {
//         if (_loadingScene) return;
//         _loadingScene = true;
//         if (SceneFader.Instance != null) {
//             ClapboardToSceneWrapper(sceneToLoad);
//         } 
//         else HardLoadScene(sceneToLoad);
//     }

//     public void FadeLoadScene(string sceneToLoad)
//     {
//         if (_loadingScene) return;
//         _loadingScene = true;
//         if (SceneFader.Instance != null) {
//             FadeToSceneWrapper(sceneToLoad);
//         } 
//         else HardLoadScene(sceneToLoad);
//     }

//     private async void FadeToSceneWrapper(string sceneToLoad)
//     {
//         try
//         {
//             await SceneFader.Instance.FadeToScene(sceneToLoad);
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"Scene transition failed: {e.Message}");
//         }
//     }

//     private async void ClapboardToSceneWrapper(string sceneToLoad)
//     {
//         try
//         {
//             await ClapboardAnimator.Instance.FadeToScene(sceneToLoad);
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"Scene transition failed: {e.Message}");
//         }
//     }

//     public void FinishLoadingScene()
//     {
//         _loadingScene = false;
//     }

//     public void HardLoadScene(string sceneToLoad)
//     {
//         Debug.Log($"Loading scene {sceneToLoad}");
//         SceneManager.LoadScene(sceneToLoad);
//         FinishLoadingScene();
//     }

//     public void QuitGame()
//     {
//         Debug.Log("Quitting Game...");
//         OnGameQuit?.Invoke();
//         Application.Quit();
//     }
    
// }
