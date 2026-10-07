using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider MouseSensitivitySlider;
    public Button CloseSettings;

    void Awake()
    {
        MouseSensitivitySlider.onValueChanged.AddListener(HandleMouseSlider);
        CloseSettings.onClick.AddListener(HandleCloseSettings);
    }

    void OnEnable()
    {
        MouseSensitivitySlider.SetValueWithoutNotify(GameSettings.MouseSensitivity);
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
