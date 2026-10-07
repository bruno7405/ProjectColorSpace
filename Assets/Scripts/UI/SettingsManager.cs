using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider MouseSensitivitySlider;
    
    void Start()
    {
        MouseSensitivitySlider.value = GameSettings.MouseSensitivity;
        
        MouseSensitivitySlider.onValueChanged.AddListener(HandleMouseSlider);
    }

    public void HandleMouseSlider(float value)
    {
        GameSettings.MouseSensitivity = value;
    }
}
