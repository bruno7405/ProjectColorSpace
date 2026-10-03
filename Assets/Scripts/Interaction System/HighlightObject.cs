using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Highlights a 3D object by fading in an overlay material.
/// The overlay material sits in its own slot and is transparent until Highlight() is called.
/// </summary>
public class HighlightObject : MonoBehaviour
{
    [SerializeField] Color highlightedColor = Color.white;
    [SerializeField, Range(0f, 1f)] float highlightAlpha = 0.25f;

    [Tooltip("Material slot that holds the highlight overlay (0 = first, 1 = second, etc.)")]
    [SerializeField] int highlightMaterialIndex = 1;

    [SerializeField] GameObject rendererParent;
    [SerializeField] private List<Renderer> renderers;

    List<Material> materials = new List<Material>();

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP/HDRP
    static readonly int ColorId = Shader.PropertyToID("_Color");         // Built-in

    private void Awake()
    {
        FindRenderers();
        FindMaterials();
        DeHighlight(); // start fully transparent
    }

    private void FindRenderers()
    {
        if (rendererParent == null) rendererParent = gameObject;

        foreach (Renderer r in rendererParent.GetComponentsInChildren<Renderer>())
        {
            if (!renderers.Contains(r))
                renderers.Add(r);
        }
    }

    private void FindMaterials()
    {
        materials.Clear();
        foreach (var renderer in renderers)
        {
            var mats = renderer.materials;

            if (highlightMaterialIndex < mats.Length)
                materials.Add(mats[highlightMaterialIndex]);
        }
    }

    public void Highlight()
    {
        SetAlpha(highlightAlpha);
    }

    public void DeHighlight()
    {
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color c = highlightedColor;
        c.a = alpha;

        foreach (var material in materials)
        {
            if (material.HasProperty(BaseColorId)) material.SetColor(BaseColorId, c);
            if (material.HasProperty(ColorId)) material.SetColor(ColorId, c);
        }
    }

    private void OnDestroy()
    {
        // Instantiated materials aren't cleaned up automatically
        foreach (var material in materials)
            if (material != null) Destroy(material);
    }
}