using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject SettingsPanel;

    public Button SettingsButton;

    void Start()
    {
        SettingsButton.onClick.AddListener(OpenSettings);
        SettingsPanel.SetActive(false);
    }

    void OnDestroy()
    {
        SettingsButton.onClick.RemoveAllListeners();
    }

    public void OpenSettings()
    {
        SettingsPanel.SetActive(true);
    }
}
