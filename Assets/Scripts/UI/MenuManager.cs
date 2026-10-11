using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject InstructionsPanel;
    public LeaderboardManager LeaderboardPanel;

    public Button SettingsButton;
    public Button InstructionsButton;
    public Button LeaderboardButton;

    void Start()
    {
        SettingsButton.onClick.AddListener(OpenSettings);
        SettingsPanel.SetActive(false);

        InstructionsPanel.SetActive(false);
        InstructionsButton.onClick.AddListener(OpenInstruct);

        LeaderboardPanel.gameObject.SetActive(false);
        LeaderboardButton.onClick.AddListener(OpenLead);

        if (SpeedrunTimer.Instance.FinishedRun)
        {
            LeaderboardPanel.gameObject.SetActive(true);
            LeaderboardPanel.AllowRunSubmission();
        }
    }

    void OnDestroy()
    {
        SettingsButton.onClick.RemoveAllListeners();
        InstructionsButton.onClick.RemoveAllListeners();
        LeaderboardButton.onClick.RemoveAllListeners();
    }

    public void OpenSettings()
    {
        SettingsPanel.SetActive(true);
    }

    public void OpenInstruct()
    {
        InstructionsPanel.SetActive(true);
    }

    public void OpenLead()
    {
        LeaderboardPanel.gameObject.SetActive(true);
    }
}
