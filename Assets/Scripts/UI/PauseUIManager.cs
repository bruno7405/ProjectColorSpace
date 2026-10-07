using UnityEngine;

public class PauseUIManager : MonoBehaviour
{
    public GameObject SettingsPanel;

    public void OpenSettings()
    {
        SettingsPanel.gameObject.SetActive(true);
    }
}
