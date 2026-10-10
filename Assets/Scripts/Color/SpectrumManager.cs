using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public static event Action<int> OnColorUnlocked;
    public static float HUE_BOUND_SIZE = 0.025f;
    public Gradient GLOBAL_GRADIENT;

    public Color RED, ORANGE, YELLOW, GREEN, BLUE, INDIGO, VIOLET;

    public int ObtainedCount = 1;

    public float MinHue => 0.5f / ColorUtilities.BANDCOUNT;
    public float MaxHue => (ObtainedCount - 0.5f) / ColorUtilities.BANDCOUNT;

    public float ScrollSpeed;
    public bool SnapToBands = false;
    private int _targetBand;
    private float _unwrappedHue;

    public float HueValue { get; private set; }

    public AudioClip turnClip;

    public static Dictionary<SpectrumColor, float> ColorVisibility = new();

    public Material BackupSpectrum, BackupEmissive, BackupSecret;
    static readonly int ObjectColorId  = Shader.PropertyToID("_Object_Color");
    static readonly int BaseColorId    = Shader.PropertyToID("_BaseColor");
    static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    
    public void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        HueValue = SpectrumColor.Red.ToValue();
        _unwrappedHue = HueValue;
        _targetBand = 0;

        BackupEmissive.EnableKeyword("_EMISSION");
        BackupEmissive.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
    }

    public void Reset()
    {
        HueValue = SpectrumColor.Red.ToValue();
        _unwrappedHue = HueValue;
        _targetBand = 0;
    }

    void Start()
    {
        OnColorUpdate?.Invoke(HueValue);
        Debug.Log("Hue: " + HueValue);
    }

    public void UnlockNextColor()
    {
        OnColorUnlocked?.Invoke(ObtainedCount);
        ObtainedCount = Mathf.Min(ObtainedCount + 1, ColorUtilities.BANDCOUNT);
    }


    public void Update()
    {
        if (SnapToBands)
        {
            SnapUpdate();
            return;
        }
        
        _unwrappedHue = HueValue;
        _targetBand = Mathf.RoundToInt(HueValue * ColorUtilities.BANDCOUNT - 0.5f);
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

    private void SnapUpdate()
    {
        int count = ColorUtilities.BANDCOUNT;

        int dir = 0;
        if (Keyboard.current.qKey.wasPressedThisFrame) {
            dir -= 1;
        }
        if (Keyboard.current.eKey.wasPressedThisFrame) {
            dir += 1;
        }

        if (dir != 0)
        {
            int previousBand = _targetBand;
            _targetBand += dir;

            if (ObtainedCount < count)
                _targetBand = Mathf.Clamp(_targetBand, 0, ObtainedCount - 1);

            if (_targetBand != previousBand)
                AudioBus.Instance.PlaySFX(turnClip);
        }

        float lastHue = HueValue;
        float targetHue = (_targetBand + 0.5f) / count;
        float diff = targetHue - _unwrappedHue;
        float step = ScrollSpeed * Time.deltaTime;

        if (Mathf.Abs(diff) <= step)
        {
            _unwrappedHue = targetHue;

            int wraps = Mathf.FloorToInt((float)_targetBand / count);
            if (wraps != 0)
            {
                _targetBand -= wraps * count;
                _unwrappedHue -= wraps;
            }
        }
        else
        {
            _unwrappedHue += Mathf.Sign(diff) * step;
        }

        HueValue = Mathf.Repeat(_unwrappedHue, 1f);

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
        SpectrumManager m = GetManager();
        if (m == null) return Color.magenta; // no manager available: obvious "missing" color

        if (color.HasFlag(SpectrumColor.Red)) return m.RED;
        if (color.HasFlag(SpectrumColor.Orange)) return m.ORANGE;
        if (color.HasFlag(SpectrumColor.Yellow)) return m.YELLOW;
        if (color.HasFlag(SpectrumColor.Green)) return m.GREEN;
        if (color.HasFlag(SpectrumColor.Blue)) return m.BLUE;
        if (color.HasFlag(SpectrumColor.Indigo)) return m.INDIGO;
        if (color.HasFlag(SpectrumColor.Violet)) return m.VIOLET;

        return Color.black;
    }

    private static SpectrumManager GetManager()
    {
        if (SpectrumManager.Instance != null) return SpectrumManager.Instance;

#if UNITY_EDITOR
        // Edit mode: Instance isn't assigned yet, so look the manager up in the scene
        if (!Application.isPlaying)
        {
            return UnityEngine.Object.FindFirstObjectByType<SpectrumManager>();
        }  
#endif
        return null;
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

    public static float CalculateVisibility(this SpectrumColor color, float hue)
    {
        float visibility = 0;

        for (int i = 0; i < ColorUtilities.Bands.Length; i++)
        {
            if ((color & ColorUtilities.Bands[i]) == 0) continue;

            float center = (i + 0.5f) / ColorUtilities.BANDCOUNT;
            visibility += ColorUtilities.Visibility(hue, center);
        }

        return Mathf.Clamp01(visibility);
    }
}