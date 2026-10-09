using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject InstructionsPanel;

    public Button SettingsButton;
    public Button InstructionsButton;

    void Start()
    {
        SettingsButton.onClick.AddListener(OpenSettings);
        SettingsPanel.SetActive(false);

        InstructionsPanel.SetActive(false);
        InstructionsButton.onClick.AddListener(OpenInstruct);
    }

    void OnDestroy()
    {
        SettingsButton.onClick.RemoveAllListeners();
        InstructionsButton.onClick.RemoveAllListeners();
    }

    public void OpenSettings()
    {
        SettingsPanel.SetActive(true);
    }

    public void OpenInstruct()
    {
        InstructionsPanel.SetActive(true);
    }
}
