using UnityEngine;

public class MenuQuit : MonoBehaviour
{
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
