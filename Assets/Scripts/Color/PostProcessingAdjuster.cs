using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingAdjuster : MonoBehaviour
{
    private VolumeProfile _profile;
    private ColorAdjustments _adjustments;
    private float _lastSaturation = float.NaN;

    void Awake()
    {
        _profile = ScriptableObject.CreateInstance<VolumeProfile>();

        _adjustments = _profile.Add<ColorAdjustments>(false);
        _adjustments.saturation.overrideState = true;

        var vol = gameObject.AddComponent<Volume>();
        vol.isGlobal = true;
        vol.priority = 100;
        vol.weight = 1f;
        vol.sharedProfile = _profile;
    }

    void Update()
    {
        float s = GameSettings.Saturation * GameSettings.SaturationOverride;
        if (s == _lastSaturation) return;

        _lastSaturation = s;
        _adjustments.saturation.value = Mathf.Lerp(-100f, 0f, Mathf.Clamp01(s));
    }

    void OnDestroy()
    {
        if (_profile != null) Destroy(_profile);
    }
}
