using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(Collider))]
public class SpectrumObject : MonoBehaviour
{
    [SerializeField] private SpectrumColor _spectrumColor;

    public float value = 1f;
    public float saturation = 1f;

    public bool StrictCollision = false;
    public bool IsSolid { get; private set; } = true;

    // Set by external systems (e.g. Keyhole) that need the body to stay kinematic
    public bool ForceKinematic { get; set; }

    public Action<bool> OnSolidChanged;

    private MeshRenderer _renderer;
    public Material _spectrum_material;
    public Material _outline_material;
    private Collider _coll;
    private Rigidbody _rb;
    private GrabbableObject _grabbableObject;

    public void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _coll = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        _grabbableObject = GetComponent<GrabbableObject>();

        // Subscribe only after references are assigned
        SpectrumManager.OnColorUpdate += HandleColorUpdate;
    }

    public void OnDestroy()
    {
        SpectrumManager.OnColorUpdate -= HandleColorUpdate;
    }

    private void HandleColorUpdate(float dial)
    {
        Apply(dial);
    }

    private void Apply(float hue)
    {
        
        Color c = ColorUtilities.FloatToColor(hue);
        c = ColorUtilities.HueToRBG(ColorUtilities.RGBtoHue(c), saturation, value);
        float v = CalculateVisibility(hue);
        _spectrum_material.SetFloat("_Ghost_Progress", 1 - v);

        if (_spectrum_material.HasProperty("_Object_Color"))
        {
            _spectrum_material.SetColor("_Object_Color", c);
        }
        else
        {
            _spectrum_material.color = c;
        }

        if (_outline_material != null)
        {
            Color color = new Color(0, 0, 0);
            color.a = v;
            _outline_material.color = color;
        }

        _renderer.enabled = v > 0.001f;

        bool newSolid = StrictCollision ? v >= 0.001f : v >= 1f;
        if (newSolid == IsSolid) return; // only touch physics on a real transition

        IsSolid = newSolid;
        _coll.enabled = IsSolid;
        RefreshKinematic();

        OnSolidChanged?.Invoke(IsSolid);
    }

    public void RefreshKinematic()
    {
        if (_rb == null) return;

        bool held = _grabbableObject != null && _grabbableObject.IsGrabbed();
        _rb.isKinematic = held || ForceKinematic || !IsSolid;
    }

    private float CalculateVisibility(float hue)
    {
        float visibility = 0;

        for (int i = 0; i < ColorUtilities.Bands.Length; i++)
        {
            if ((_spectrumColor & ColorUtilities.Bands[i]) == 0) continue;

            float center = (i + 0.5f) / ColorUtilities.BANDCOUNT;
            visibility += ColorUtilities.Visibility(hue, center);
        }

        return Mathf.Clamp01(visibility);
    }
}