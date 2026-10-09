using System;
using UnityEngine;

public class SpectrumObject : MonoBehaviour
{
    [SerializeField] private SpectrumColor _spectrumColor;

    public float value = 1f;
    public float saturation = 1f;
    public float emission = 0;

    public bool StrictCollision = false;
    public bool IsSolid { get; private set; } = true;

    // Set by external systems (e.g. Keyhole) that need the body to stay kinematic
    public bool ForceKinematic { get; set; }

    public Action<bool> OnSolidChanged;

    private MeshRenderer _renderer;
    private Material _spectrum_material;
    private Material _outline_material;
    private Collider _coll;
    private Rigidbody _rb;
    private GrabbableObject _grabbableObject;

    private bool _cacheHasColorProperty;
    private bool _cacheHasEmissionProperty;

    private float _fadeSpeed = 0.75f;
    private float _heldMinVis = 0.5f;
    private float _dialVisibility;
    private float _actualV;
    private bool _wasHeld;
    private bool _dirty = true;
    private Color _color;

    private bool _inheritedFromParent;

    public SpectrumColor GetSpectrumColor() { return _spectrumColor; }

    public void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        if (_renderer == null) return;
        _renderer.SetPropertyBlock(null);
        _coll = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        _grabbableObject = GetComponent<GrabbableObject>();

        _spectrum_material = _renderer.materials[0];
        _cacheHasColorProperty = _spectrum_material.HasProperty("_Object_Color");
        _cacheHasEmissionProperty = _spectrum_material.HasProperty("_Emission");

        InheritFromParent();

        // Subscribe only after references are assigned
        SpectrumManager.OnColorUpdate += HandleColorUpdate;
    }

    public void InheritFromParent()
    {
        if (_inheritedFromParent) return;
        _inheritedFromParent = true;

        if (transform.parent == null) return;

        SpectrumObject parent = transform.parent.GetComponent<SpectrumObject>();
        if (parent == null) return;

        // Make sure the parent has resolved its own inheritance first,
        // so nested chains (grandparent -> parent -> child) work in any Awake order.
        parent.InheritFromParent();

        _spectrumColor = parent._spectrumColor;
        StrictCollision = parent.StrictCollision;
        _fadeSpeed = parent._fadeSpeed;
        _heldMinVis = parent._heldMinVis;
    }

    public void Start()
    {
        if (_renderer == null) return;
        
        UpdateDial(SpectrumManager.Instance.HueValue);
        _actualV = GetTarget();
        _wasHeld = IsHeld();
        ApplyVisuals();
    }

    public void OnDestroy()
    {
        SpectrumManager.OnColorUpdate -= HandleColorUpdate;
    }

    private void HandleColorUpdate(float dial)
    {
        UpdateDial(dial);
        _actualV = GetTarget();
        _dirty = true;
    }

    private void UpdateDial(float hue)
    {
        Color c = ColorUtilities.FloatToColor(hue);
        _color = ColorUtilities.Shade(c, saturation, value);
        //_color = ColorUtilities.HueToRBG(ColorUtilities.RGBtoHue(c), saturation, value);
        _dialVisibility = CalculateVisibility(hue);
    }

    private bool IsHeld() => _grabbableObject != null && _grabbableObject.IsGrabbed();

    private float GetTarget()
    {
        return IsHeld() ? Mathf.Max(_heldMinVis, _dialVisibility) : _dialVisibility;
    }

    public void Update()
    {
        bool held = IsHeld();
        if (held != _wasHeld)
        {
            _wasHeld = held;
            RefreshKinematic();
        }

        float target = GetTarget();
        if (!Mathf.Approximately(_actualV, target))
        {
            _actualV = Mathf.MoveTowards(_actualV, target, _fadeSpeed * Time.deltaTime);
            _dirty = true;
        }

        if (_dirty)
        {
            _dirty = false;
            ApplyVisuals();
        }
    }

    private void ApplyVisuals()
    {
        if (_renderer == null) return;

        _spectrum_material.SetFloat("_Ghost_Progress", 1 - _actualV);

        if (_cacheHasColorProperty) _spectrum_material.SetColor("_Object_Color", _color);
        else _spectrum_material.color = _color;

        if (emission != 0 && _cacheHasEmissionProperty) _spectrum_material.SetFloat("_Emission", emission);

        if (_outline_material != null)
        {
            _outline_material.color = new Color(0, 0, 0, 1 - _actualV);
        }

        _renderer.enabled = _actualV > 0.001f;

        bool newSolid = StrictCollision ? _actualV >= 0.001f : _actualV >= 0.99f;
        if (newSolid == IsSolid) return;

        IsSolid = newSolid;
        if (_coll != null) _coll.enabled = IsSolid;
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

#if UNITY_EDITOR
    private static MaterialPropertyBlock _previewBlock;

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        ApplyEditorPreview();
    }

    private void Reset()
    {
        ApplyEditorPreview();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
        {
            var r = GetComponent<MeshRenderer>();
            if (r != null) r.SetPropertyBlock(null);
        }
    }

    private static readonly Color EditorPreviewColor = new Color(0.9f, 0.3f, 0.2f);

    private void ApplyEditorPreview()
    {
        var r = GetComponent<MeshRenderer>();
        if (r == null) return;

        _previewBlock ??= new MaterialPropertyBlock();
        r.GetPropertyBlock(_previewBlock);

        Color shaded = ColorUtilities.Shade(EditorPreviewColor, saturation, value);

        _previewBlock.SetColor("_Object_Color", shaded);
        _previewBlock.SetColor("_Color", shaded);
        _previewBlock.SetFloat("_Ghost_Progress", 0f);

        r.SetPropertyBlock(_previewBlock);
    }

    private float GetPreviewHue()
    {
        SpectrumObject source = this;
        while (source.transform.parent != null)
        {
            var parent = source.transform.parent.GetComponent<SpectrumObject>();
            if (parent == null) break;
            source = parent;
        }

        for (int i = 0; i < ColorUtilities.Bands.Length; i++)
        {
            if ((source._spectrumColor & ColorUtilities.Bands[i]) != 0)
                return (i + 0.5f) / ColorUtilities.BANDCOUNT;
        }
        return -1f;
    }
#endif
}