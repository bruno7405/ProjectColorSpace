using System.Collections.Generic;
using UnityEngine;

public class PrismCollector : MonoBehaviour
{
    [SerializeField] SpectrumColor prismColor;
    [SerializeField] ParticleSystem _auraParticles;
    [SerializeField] ParticleSystem _burstParticles;

    public AudioClip prismBreak;

    private Material colorMaterial;

    private void Awake()
    {
        colorMaterial = Resources.Load<Material>($"Materials/Colors/{prismColor.ToString()}Glow");
        if (colorMaterial != null)
        {
            _auraParticles.GetComponent<ParticleSystemRenderer>().material = colorMaterial;
        }
        else
        {
            Debug.LogWarning($"No material found at Resources/Materials/{prismColor}", this);
        }
    }

    public void CollectPrism()
    {
        ParticleSystem particle = Instantiate(_burstParticles, transform.position, Quaternion.identity);
        particle.GetComponent<ParticleSystemRenderer>().material = colorMaterial;

        SpectrumManager.Instance.UnlockNextColor();

        AudioBus.Instance.PlaySFX(prismBreak);

        Destroy(gameObject);
    }
}
