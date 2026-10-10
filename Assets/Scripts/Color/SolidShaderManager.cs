using UnityEngine;

public class SolidShaderManager : MonoBehaviour
{
    public MaterialSVSet[] MaterialSets;

     private void OnEnable()
    {
        SpectrumManager.OnColorUpdate += HandleColorUpdate;
    }

    private void OnDisable()
    {
        SpectrumManager.OnColorUpdate -= HandleColorUpdate;
    }

    private void HandleColorUpdate(float hue)
    {
        Color c = ColorUtilities.FloatToColor(hue);
        
        foreach (var set in MaterialSets)
        {
            set.TargetMaterial.color = ColorUtilities.Shade(c, set.Saturation, set.Value);
        }
    }
}

[System.Serializable]
public class MaterialSVSet
{
    public Material TargetMaterial;
    public float Saturation;
    public float Value;
}