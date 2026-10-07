using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider MouseSensitivitySlider;
    public Button CloseSettings;
    
    void Start()
    {
        MouseSensitivitySlider.value = GameSettings.MouseSensitivity;

        MouseSensitivitySlider.onValueChanged.AddListener(HandleMouseSlider);
        CloseSettings.onClick.AddListener(HandleCloseSettings);
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
