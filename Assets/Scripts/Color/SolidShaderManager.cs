using UnityEngine;

public class SolidShaderManager : MonoBehaviour
{
    public MaterialSVSet[] MaterialSets;

    static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    static readonly int ObjectColorId = Shader.PropertyToID("_Object_Color");

    private void OnEnable()
    {
        SpectrumManager.OnColorUpdate += HandleColorUpdate;
    }

    private void OnDisable()
    {
        SpectrumManager.OnColorUpdate -= HandleColorUpdate;
    }

    void Start()
    {
        foreach (var set in MaterialSets)
        {
            if (set.TargetMaterial == null || !set.Emissive) continue;
            set.TargetMaterial.EnableKeyword("_EMISSION");
            set.TargetMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }

        HandleColorUpdate(SpectrumManager.Instance.HueValue);
    }

    private void HandleColorUpdate(float hue)
    {
        Color c = ColorUtilities.FloatToColor(hue);

        foreach (var set in MaterialSets)
        {
            if (set.TargetMaterial == null) continue;

            Color shaded = ColorUtilities.Shade(c, set.Saturation, set.Value);
            set.TargetMaterial.color = shaded;

            if (set.Emissive)
                set.TargetMaterial.SetColor(ObjectColorId, shaded);
        }
    }
}

[System.Serializable]
public class MaterialSVSet
{
    public Material TargetMaterial;
    public float Saturation;
    public float Value;
    public bool Emissive;
    public float EmissionIntensity = 1f;
}