using UnityEngine;

public class MenuPlay : MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.Instance.StartGame();
    }
}
