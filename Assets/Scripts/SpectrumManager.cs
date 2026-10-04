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
    private static float _spectrumIndex = 1f / ColorUtilities.BANDCOUNT;
    public static float HUE_BOUND_SIZE = 0.025f;
    public Gradient GLOBAL_GRADIENT;

    public float ScrollSpeed;

    public float _hueValue = 0f;
    public float _calculatedHueValue;

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
        float lastHue = _hueValue;
        
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            _spectrumIndex -= ScrollSpeed * Time.deltaTime;
            _hueValue -= ScrollSpeed * Time.deltaTime;
            _hueValue = Mathf.Repeat(_hueValue, 1f);
        } 
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            _spectrumIndex += ScrollSpeed * Time.deltaTime;
            _hueValue += ScrollSpeed * Time.deltaTime;
            _hueValue = Mathf.Repeat(_hueValue, 1f);
        }
    
        _hueValue = ((_hueValue % 1) + 1) % 1;

        if (_hueValue != lastHue)
        {
            OnColorUpdate?.Invoke(_hueValue);
        }

        Debug.Log("Index: " + _hueValue);
    }
}