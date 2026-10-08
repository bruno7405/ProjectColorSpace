using System;
using UnityEngine;
using UnityEngine.VFX;

public class SpectrumVFX : MonoBehaviour
{
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private SpectrumObject spectrumObject;
    SpectrumColor allowedColors;
    float baseAlpha = 1f;

    void OnHueChanged(float h)
    {
        visualEffect.SetFloat("SpectrumHue", h);

        // if hue is going out of range, set alpha
        // TODO
        // In the range that the allowed colors are, it should be full alpha.
        // Otherwise, just set to full transparent when out of range
        bool allowed = false;

        foreach (SpectrumColor color in Enum.GetValues(typeof(SpectrumColor)))
        {
            if (color == SpectrumColor.None) { 
                //Debug.Log("=== None");
                continue;
            }

            if (allowedColors.HasFlag(color))
            {
                float colorHue = color.ToValue();

                if (Mathf.Abs(ColorUtilities.HueDifference(h, colorHue))
                    <= SpectrumManager.HUE_BOUND_SIZE)
                {
                    //Debug.Log("=== Allowed");
                    allowed = true;
                    break;
                }
                //Debug.Log("=== Not Allowed");
            }
        }

        visualEffect.SetFloat("BaseAlpha", allowed ? baseAlpha : 0f);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpectrumManager.OnColorUpdate += OnHueChanged;
        allowedColors = spectrumObject.GetSpectrumColor();
    }

    void OnDisable()
    {
        SpectrumManager.OnColorUpdate -= OnHueChanged;
    }
}
