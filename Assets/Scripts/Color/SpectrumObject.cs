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

    private Renderer _renderer;
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

    static readonly int GhostId    = Shader.PropertyToID("_Ghost_Progress");
    static readonly int ColorId    = Shader.PropertyToID("_Object_Color");
    static readonly int EmissionId = Shader.PropertyToID("_Emission");
    private float _hue;

    public bool SecretWall = false;


    public void Awake()
    {
        _coll = GetComponent<Collider>();
        
        _renderer = GetComponent<Renderer>();
        //if( _renderer == null ) { _renderer = GetComponent<SkinnedMeshRenderer>(); }
        if (_renderer != null) _renderer.SetPropertyBlock(null);
        
        
        _rb = GetComponent<Rigidbody>();
        _grabbableObject = GetComponent<GrabbableObject>();

        if (_renderer != null) _spectrum_material = _renderer.materials[0];
        if (_renderer != null) _cacheHasColorProperty = _spectrum_material.HasProperty("_Object_Color");
        if (_renderer != null) _cacheHasEmissionProperty = _spectrum_material.HasProperty("_Emission");

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
        //UpdateDial(SpectrumManager.Instance.HueValue);
        _actualV = GetTarget();
        _wasHeld = IsHeld();
        ApplyVisuals();
    }

    public void OnDestroy()
    {
        SpectrumManager.OnColorUpdate -= HandleColorUpdate;
    }

    private void HandleColorUpdate(float hue)
    {
        _hue = hue;
        _dialVisibility = CalculateVisibility(hue);

        if (_dialVisibility <= 0.001f && _actualV <= 0.001f && !_wasHeld) return;

        _actualV = GetTarget();
        _dirty = true;
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
        bool visible = _actualV > 0.001f;
        if (_renderer != null)
        {
            _renderer.enabled = visible;
            if (visible)
            {
                if (SecretWall)_color = ColorUtilities.Shade(ColorUtilities.FloatToColor(_hue), 0.75f, 0.8f);
                else _color = ColorUtilities.Shade(ColorUtilities.FloatToColor(_hue), 0.95f, 0.8f);

                //_color = ColorUtilities.Shade(ColorUtilities.FloatToColor(_hue), 0.95f, 0.8f); // should standardize this with a flag probably!!!
                _spectrum_material.SetFloat(GhostId, 1 - _actualV);
                if (_cacheHasColorProperty) _spectrum_material.SetColor(ColorId, _color);
                else _spectrum_material.color = _color;
                if (emission != 0 && _cacheHasEmissionProperty) _spectrum_material.SetFloat(EmissionId, emission);
            }
        }
        

        if (_outline_material != null)
        {
            _outline_material.color = new Color(0, 0, 0, 1 - _actualV);
        }
        

        if (_renderer != null) _renderer.enabled = _actualV > 0.001f;

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

// #if UNITY_EDITOR
//     private static MaterialPropertyBlock _previewBlock;

//     private void OnValidate()
//     {
//         if (Application.isPlaying) return;
//         ApplyEditorPreview();
//     }

//     private void Reset()
//     {
//         ApplyEditorPreview();
//     }

//     private void OnDisable()
//     {
//         if (!Application.isPlaying)
//         {
//             var r = GetComponent<Renderer>();
//             if (r != null) r.SetPropertyBlock(null);
//         }
//     }

//     private static readonly Color EditorPreviewColor = new Color(0.9f, 0.3f, 0.2f);

//     private void ApplyEditorPreview()
//     {
//         var r = GetComponent<Renderer>();
//         if (r == null) return;

//         _previewBlock ??= new MaterialPropertyBlock();
//         r.GetPropertyBlock(_previewBlock);

//         Color shaded = ColorUtilities.Shade(EditorPreviewColor, saturation, value);

//         _previewBlock.SetColor("_Object_Color", shaded);
//         _previewBlock.SetColor("_Color", shaded);
//         _previewBlock.SetFloat("_Ghost_Progress", 0f);

//         r.SetPropertyBlock(_previewBlock);
//     }

//     private float GetPreviewHue()
//     {
//         SpectrumObject source = this;
//         while (source.transform.parent != null)
//         {
//             var parent = source.transform.parent.GetComponent<SpectrumObject>();
//             if (parent == null) break;
//             source = parent;
//         }

//         for (int i = 0; i < ColorUtilities.Bands.Length; i++)
//         {
//             if ((source._spectrumColor & ColorUtilities.Bands[i]) != 0)
//                 return (i + 0.5f) / ColorUtilities.BANDCOUNT;
//         }
//         return -1f;
//     }
// #endif
}