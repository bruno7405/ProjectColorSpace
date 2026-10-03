using System.Runtime.CompilerServices;
using UnityEngine;

public class SpectrumObject : MonoBehaviour
{
    [SerializeField] private SpectrumColor _spectrumColor;
    [SerializeField] private Gradient _gradient;

    public float value = 1f;
    public float saturation = 1f;

    private MeshRenderer _renderer;
    private Material _material;
    private Color _baseColor;
    private Collider _coll;

    public void Awake()
    {
        SpectrumManager.OnColorUpdate += HandleColorUpdate;
    }

    public void OnDestroy()
    {
        SpectrumManager.OnColorUpdate -= HandleColorUpdate;
    }

    public void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _material = _renderer.material;
        _renderer.material.color = _spectrumColor.ToColor();
        _baseColor = _spectrumColor.ToColor();
        _coll = GetComponent<Collider>();
    }

    private void HandleColorUpdate(float dial)
    {
        Apply(dial);
    }

    private void Apply(float dial)
    {
        float _currentHue = SpectrumManager.Instance._hueValue;
        float v = 1f;

        Color c = SpectrumManager.Instance.GLOBAL_GRADIENT.Evaluate(_currentHue);
        c.a = v;
        _material.color = c;

        _renderer.enabled = v > 0.001f;

        if (v < 1f)
        {
            _coll.enabled = false;
        }
        else
        {
            _coll.enabled = true;
        }
    }

}
