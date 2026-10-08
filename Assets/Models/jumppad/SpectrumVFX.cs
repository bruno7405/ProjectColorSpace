using UnityEngine;
using UnityEngine.VFX;

public class SpectrumVFX : MonoBehaviour
{
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private SpectrumObject spectrumObject;
    float baseAlpha = 1f;

    void OnHueChanged(float h)
    {
        visualEffect.SetFloat("SpectrumHue", h);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpectrumManager.OnColorUpdate += OnHueChanged;
    }

    void OnDisable()
    {
        SpectrumManager.OnColorUpdate -= OnHueChanged;
    }
}
