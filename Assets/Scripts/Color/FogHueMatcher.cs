using System.Reflection;
using UnityEngine;
using FlatKit;

public class FogHueMatcher : MonoBehaviour
{
    [SerializeField] private FogSettings _fog;
    [Range(0f, 1.5f)] [SerializeField] private float _saturationScale = 0.8f; 
    [SerializeField] private float _smoothing = 3f;
    public float HueOffset = 0.1f;

    private Gradient _origDistance, _origHeight;
    private Gradient _workDistance = new Gradient();
    private Gradient _workHeight = new Gradient();

    private Color _targetColor, _currentColor;
    private float _appliedHue = -1f;
    private MethodInfo _onValidate;

    private void Awake()
    {
        _origDistance = Clone(_fog.distanceGradient);
        _origHeight = Clone(_fog.heightGradient);
        _onValidate = typeof(FogSettings).GetMethod("OnValidate",
            BindingFlags.NonPublic | BindingFlags.Instance);
    }

    private void OnEnable()  => SpectrumManager.OnColorUpdate += OnDial;
    private void OnDisable() => SpectrumManager.OnColorUpdate -= OnDial;

    private void Start()
    {
        OnDial(SpectrumManager.Instance.HueValue);
        _currentColor = _targetColor;
        Apply(true);
    }

    private void OnDestroy()
    {
        if (_fog == null) return;
        _fog.distanceGradient = _origDistance;
        _fog.heightGradient = _origHeight;
        Notify();
    }

    private void OnDial(float dial)
    {
        _targetColor = ColorUtilities.FloatToColor(dial);
    }

    private void Update()
    {
        _currentColor = Color.Lerp(_currentColor, _targetColor,
            1f - Mathf.Exp(-_smoothing * Time.deltaTime));
        Apply(false);
    }

    private void Apply(bool force)
    {
        Color.RGBToHSV(_currentColor, out float dialHue, out _, out _);
        float hue = Mathf.Repeat(dialHue + HueOffset, 1f);

        if (!force && Mathf.Abs(Mathf.DeltaAngle(hue * 360f, _appliedHue * 360f)) < 0.5f) return;
        _appliedHue = hue;

        Retint(_origDistance, _workDistance, hue);
        Retint(_origHeight, _workHeight, hue);
        _fog.distanceGradient = _workDistance;
        _fog.heightGradient = _workHeight;
        Notify();
    }

    private void Retint(Gradient src, Gradient dst, float hue)
    {
        var keys = src.colorKeys;
        for (int i = 0; i < keys.Length; i++)
        {
            Color.RGBToHSV(keys[i].color, out _, out float s, out float v);
            keys[i].color = Color.HSVToRGB(hue, Mathf.Clamp01(s * _saturationScale), v);
        }
        dst.SetKeys(keys, src.alphaKeys);
    }

    private void Notify() => _onValidate?.Invoke(_fog, null);

    private static Gradient Clone(Gradient g)
    {
        var c = new Gradient();
        c.SetKeys(g.colorKeys, g.alphaKeys);
        c.mode = g.mode;
        return c;
    }
}