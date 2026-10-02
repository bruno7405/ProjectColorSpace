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

    public float ScrollSpeed;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public void Update()
    {
        float lastIndex = _spectrumIndex;
        
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            _spectrumIndex -= ScrollSpeed * Time.deltaTime;
        } 
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            _spectrumIndex += ScrollSpeed * Time.deltaTime;
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
    private const int BANDCOUNT = 7; 
    
    public static float Center(this SpectrumColor color)
    {
        return ((int) color + 0.5f) / BANDCOUNT;
    }
}