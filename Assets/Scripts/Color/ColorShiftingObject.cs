using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ColorShiftingObject : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    
    public void Awake()
    {
        SpectrumManager.OnColorUpdate += HandleColorUpdate;

        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void OnDestroy()
    {
        SpectrumManager.OnColorUpdate -= HandleColorUpdate;
    }

    public void Start()
    {
        
    }

    private void HandleColorUpdate(float hue)
    {
        _meshRenderer.material.color = ColorUtilities.FloatToColor(hue);
    }
}
