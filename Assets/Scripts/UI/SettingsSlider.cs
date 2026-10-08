using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSlider : MonoBehaviour
{
    public Slider MySlider;
    public TMP_Text ValueText;

    public enum SliderType { MouseSense, Saturation, Music, SFX }

    [SerializeField] private SliderType _sliderType;

    void Awake()
    {
        MySlider.onValueChanged.AddListener(HandleSlider);
    }

    void OnEnable()
    {
        switch (_sliderType)
        {
            case SliderType.MouseSense:
                MySlider.SetValueWithoutNotify(GameSettings.MouseSensitivity);
                break;
            case SliderType.Saturation:
                MySlider.SetValueWithoutNotify(GameSettings.Saturation);
                break;
            case SliderType.Music:
                MySlider.SetValueWithoutNotify(GameSettings.MusicVolume);
                break;
            case SliderType.SFX:
                MySlider.SetValueWithoutNotify(GameSettings.SFXVolume);
                break;
        }

        ValueText.text = MySlider.value.ToString("F2");
    }

    public void HandleSlider(float value)
    {
        switch (_sliderType)
        {
            case SliderType.MouseSense:
                GameSettings.MouseSensitivity = value;
                break;
            case SliderType.Saturation:
                GameSettings.Saturation = value;
                break;
            case SliderType.Music:
                GameSettings.MusicVolume = value;
                break;
            case SliderType.SFX:
                GameSettings.SFXVolume = value;
                break;
        }
        
        ValueText.text = value.ToString("F2");
    }

}
