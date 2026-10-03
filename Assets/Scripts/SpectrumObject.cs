using UnityEngine;

public class SpectrumObject : MonoBehaviour
{
    [SerializeField] private SpectrumColor _spectrumColor;

    private MeshRenderer _renderer;
    private Material _material;
    private Color _baseColor;

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
    }

    private void HandleColorUpdate(float dial)
    {
        Apply(dial);
    }

    private void Apply(float dial)
    {
        float v = ColorUtilities.Visibility(dial, _spectrumColor.Center());

        Color c = _baseColor;
        c.a = v;
        _material.color = c;

        _renderer.enabled = v > 0.001f;
    }
}
