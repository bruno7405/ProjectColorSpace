using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject SettingsPanel;

    public Button SettingsButton, CloseButton;

    void Start()
    {
        SettingsButton.onClick.AddListener(() => OpenSettings(true));
        CloseButton.onClick.AddListener(() => OpenSettings(false));
        SettingsPanel.SetActive(false);
    }

    void OnDestroy()
    {
        SettingsButton.onClick.RemoveAllListeners();
        CloseButton.onClick.RemoveAllListeners();
    }

    public void OpenSettings(bool open)
    {
        SettingsPanel.SetActive(open);
    }
}
