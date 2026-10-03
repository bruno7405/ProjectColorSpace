using UnityEngine;

public class ColorShifter : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    
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
       _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void HandleColorUpdate(float color)
    {
        _meshRenderer.material.color = ColorUtilities.FloatToColor(color);
    }
}
