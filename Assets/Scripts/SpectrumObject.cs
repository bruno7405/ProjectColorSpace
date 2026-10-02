using UnityEngine;

public class SpectrumObject : MonoBehaviour
{
    [SerializeField] private SpectrumColor _spectrumColor;

    public void Awake()
    {
        SpectrumManager.OnColorUpdate += HandleColorUpdate;
    }

    public void OnDestroy()
    {
        SpectrumManager.OnColorUpdate -= HandleColorUpdate;
    }

    private void HandleColorUpdate(float color)
    {
        
    }
}
