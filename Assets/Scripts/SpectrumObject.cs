using System.Runtime.CompilerServices;
using UnityEngine;

public class SpectrumObject : MonoBehaviour
{
    [SerializeField] private SpectrumColor _spectrumColor;

    public float value = 1f;
    public float saturation = 1f;

    public bool StrictCollision = false;

    private MeshRenderer _renderer;
    private Material _material;
    private Collider _coll;
    private Rigidbody _rb;

    private GrabbableObject _grabbableObject;

    public void Awake()
    {
        SpectrumManager.OnColorUpdate += HandleColorUpdate;

        _renderer = GetComponent<MeshRenderer>();
        _material = _renderer.material;
        _renderer.material.color = _spectrumColor.ToColor();
        _coll = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
    }

    public void OnDestroy()
    {
        SpectrumManager.OnColorUpdate -= HandleColorUpdate;
    }

    public void Start()
    {
        
    }

    private void HandleColorUpdate(float dial)
    {
        Apply(dial);
    }

    private void Apply(float hue)
    {
        Color c = ColorUtilities.FloatToColor(hue);
        float v = CalculateVisibility(hue);
        c.a = v;

        _material.color = c;
        _renderer.enabled = v > 0.001f;

        if ((!StrictCollision && v < 1f) || (StrictCollision && v < 0.001f))
        {
            _coll.enabled = false;
            if (_rb != null && _grabbableObject == null)
            {
                //_rb.isKinematic = true;
            }
        }
        else
        {
            _coll.enabled = true;
            if (_rb != null && _grabbableObject == null)
            {
                //_rb.isKinematic = false;
            }
        }
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
