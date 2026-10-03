using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SpectrumColor 
{ 
    Red = 0, 
    Orange = 1, 
    Yellow = 2, 
    Green = 3, 
    Blue = 4, 
    Indigo = 5, 
    Violet = 6 
}

public class SpectrumManager : MonoBehaviour
{
    public static SpectrumManager Instance;
    public static event Action<float> OnColorUpdate;
    private static float _spectrumIndex = SpectrumColor.Red.Center();

    public Color RED, ORANGE, YELLOW, GREEN, BLUE, INDIGO, VIOLET;

    public float ScrollSpeed;

    public float _hueValue = 0f;
    public Color _shiftedHue;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        OnColorUpdate?.Invoke(_spectrumIndex);
    }

    public void Update()
    {
        float lastIndex = _spectrumIndex;
        
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            _spectrumIndex -= ScrollSpeed * Time.deltaTime;
            _hueValue -= ScrollSpeed * Time.deltaTime;
            _hueValue = Mathf.Repeat(_hueValue, 1f);
            _shiftedHue = ColorUtilities.HueToRBG(_hueValue);
        } 
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            _spectrumIndex += ScrollSpeed * Time.deltaTime;
            _hueValue += ScrollSpeed * Time.deltaTime;
            _hueValue = Mathf.Repeat(_hueValue, 1f);
            _shiftedHue = ColorUtilities.HueToRBG(_hueValue);
        }
    
        _spectrumIndex = ((_spectrumIndex % 1) + 1) % 1;

        if (_spectrumIndex != lastIndex)
        {
            OnColorUpdate?.Invoke(_spectrumIndex);
        }

        Debug.Log("Index: " + _spectrumIndex);
    }
}

public static class SpectrumColorExtensions
{
    public static float Center(this SpectrumColor color)
    {
        return ((int) color + 0.5f) / ColorUtilities.BANDCOUNT;
    }

    public static Color ToColor(this SpectrumColor color)
    {
        switch (color)
        {
            case SpectrumColor.Red: return SpectrumManager.Instance.RED;
            case SpectrumColor.Orange: return SpectrumManager.Instance.ORANGE;
            case SpectrumColor.Yellow: return SpectrumManager.Instance.YELLOW;
            case SpectrumColor.Green: return SpectrumManager.Instance.GREEN;
            case SpectrumColor.Blue: return SpectrumManager.Instance.BLUE;
            case SpectrumColor.Indigo: return SpectrumManager.Instance.INDIGO;
            case SpectrumColor.Violet: return SpectrumManager.Instance.VIOLET;
            default: return Color.black;
        }
    }
}