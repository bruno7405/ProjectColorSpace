using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider MouseSensitivitySlider;
    public Button CloseSettings;

    void Awake()
    {
        MouseSensitivitySlider.value = GameSettings.MouseSensitivity;

        MouseSensitivitySlider.onValueChanged.AddListener(HandleMouseSlider);
        CloseSettings.onClick.AddListener(HandleCloseSettings);
    }

    void Start()
    {
        
    }

    public void HandleMouseSlider(float value)
    {
        GameSettings.MouseSensitivity = value;
    }

    public void HandleCloseSettings()
    {
        gameObject.SetActive(false);
    }
}
