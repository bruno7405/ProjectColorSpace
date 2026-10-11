using UnityEngine;
using UnityEngine.UI;

public class SpeedruninngModeButton : MonoBehaviour
{
    public Button SpeedrunButton;
    public Image CheckImage;

    void Awake()
    {
        SpeedrunButton.onClick.AddListener(ToggleSpeedrunMode);
    }

    void OnEnable()
    {
        CheckImage.enabled = GameSettings.SpeedrunMode;
    }

    void OnDestroy()
    {
        SpeedrunButton.onClick.RemoveListener(ToggleSpeedrunMode);
    }

    public void ToggleSpeedrunMode()
    {
        GameSettings.SpeedrunMode = !GameSettings.SpeedrunMode;
        CheckImage.enabled = GameSettings.SpeedrunMode;

        GameSettings.OnSettingsChanged?.Invoke();
    }
}
