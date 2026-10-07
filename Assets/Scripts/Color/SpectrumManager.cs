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

    public int ObtainedCount = 1;

    public float MinHue => 0.5f / ColorUtilities.BANDCOUNT;
    public float MaxHue => (ObtainedCount - 0.5f) / ColorUtilities.BANDCOUNT;

    public float ScrollSpeed;

    public float HueValue { get; private set; }
    
    public void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        HueValue = SpectrumColor.Red.ToValue();
    }

    void Start()
    {
        OnColorUpdate?.Invoke(HueValue);
        Debug.Log("Hue: " + HueValue);
    }

    public void UnlockNextColor()
    {
        ObtainedCount = Mathf.Min(ObtainedCount + 1, ColorUtilities.BANDCOUNT);
    }

    public void Update()
    {
        float lastHue = HueValue;
        
        float input = 0f;
        if (Keyboard.current.qKey.isPressed) input -= 1f;
        if (Keyboard.current.eKey.isPressed) input += 1f;

        float next = HueValue + input * ScrollSpeed * Time.deltaTime;

        HueValue = ObtainedCount < ColorUtilities.BANDCOUNT
            ? Mathf.Clamp(next, MinHue, MaxHue)
            : Mathf.Repeat(next, 1f);

        if (HueValue != lastHue)
        {
            OnColorUpdate?.Invoke(HueValue);
        }
    }

    public void ForceColorUpdate()
    {
        OnColorUpdate?.Invoke(HueValue);
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