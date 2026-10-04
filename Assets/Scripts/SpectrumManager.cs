using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Flags]
public enum SpectrumColor 
{ 
    None = 0,
    Red = 1 << 0, 
    Orange = 1 << 1, 
    Yellow = 1 << 2, 
    Green = 1 << 3, 
    Blue = 1 << 4, 
    Indigo = 1 << 5, 
    Violet = 1 << 6
}

public class SpectrumManager : MonoBehaviour
{
    public static SpectrumManager Instance;
    public static event Action<float> OnColorUpdate;
    public static float HUE_BOUND_SIZE = 0.025f;
    public Gradient GLOBAL_GRADIENT;

    public Color RED, ORANGE, YELLOW, GREEN, BLUE, INDIGO, VIOLET;

    public float ScrollSpeed;

    [NonSerialized] public float _hueValue;
    
    public void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        _hueValue = SpectrumColor.Red.ToValue();
    }

    void Start()
    {
        OnColorUpdate?.Invoke(_hueValue);
    }

    public void Update()
    {
        float lastHue = _hueValue;
        
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            _hueValue -= ScrollSpeed * Time.deltaTime;
            _hueValue -= ScrollSpeed * Time.deltaTime;
            _hueValue = Mathf.Repeat(_hueValue, 1f);
        } 
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            _hueValue += ScrollSpeed * Time.deltaTime;
            _hueValue += ScrollSpeed * Time.deltaTime;
            _hueValue = Mathf.Repeat(_hueValue, 1f);
        }
    
        _hueValue = ((_hueValue % 1) + 1) % 1;

        if (_hueValue != lastHue)
        {
            OnColorUpdate?.Invoke(_hueValue);
        }

        Debug.Log("Hue: " + _hueValue);
    }
}

public static class SpectrumColorExtensions
{
    public static Color ToColor(this SpectrumColor color)
    {
        if (color.HasFlag(SpectrumColor.Red))       return SpectrumManager.Instance.RED;
        if (color.HasFlag(SpectrumColor.Orange))    return SpectrumManager.Instance.ORANGE;
        if (color.HasFlag(SpectrumColor.Yellow))    return SpectrumManager.Instance.YELLOW;
        if (color.HasFlag(SpectrumColor.Green))     return SpectrumManager.Instance.GREEN;
        if (color.HasFlag(SpectrumColor.Blue))      return SpectrumManager.Instance.BLUE;
        if (color.HasFlag(SpectrumColor.Indigo))    return SpectrumManager.Instance.INDIGO;
        if (color.HasFlag(SpectrumColor.Violet))    return SpectrumManager.Instance.VIOLET;
        
        return Color.black;
    }

    public static float ToValue(this SpectrumColor color)
    {
        int c = 0;

        if (color.HasFlag(SpectrumColor.Red))            c = 0;
        else if (color.HasFlag(SpectrumColor.Orange))    c = 1;
        else if (color.HasFlag(SpectrumColor.Yellow))    c = 2;
        else if (color.HasFlag(SpectrumColor.Green))     c = 3;
        else if (color.HasFlag(SpectrumColor.Blue))      c = 4;
        else if (color.HasFlag(SpectrumColor.Indigo))    c = 5;
        else if (color.HasFlag(SpectrumColor.Violet))    c = 6;
        
        return (c + 0.5f) / ColorUtilities.BANDCOUNT;
    }
}