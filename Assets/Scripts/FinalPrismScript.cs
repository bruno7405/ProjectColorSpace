using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class FinalPrismScript : MonoBehaviour
{
    [SerializeField] private GameObject prismGameObject;
    private Material prismMaterial;
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private VisualEffectAsset destroyVFX;
    [SerializeField] private GameObject[] effectsToDestroyBeforePlayingVFX;


    bool startCountdown = false;
    [SerializeField] private float countdownToDestroy = 12.00f; // wanna destroy right when/after he says i got roygbiv
    float currentBloom = 3;
    float BLOOM_INCREASE_RATE = 250.0f / 12.00f;
    float SHRINK_RATE = 0.07f;
    bool destroyedList = false;
    bool madeParticles = false;

    public void StartDestroyPrism()
    {
        startCountdown = true;
        Debug.Log("Start destroy prism");
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prismMaterial = targetRenderer.material;
    }

     void Update()
    {
        if(startCountdown)
        {
            countdownToDestroy -= Time.deltaTime;
            currentBloom += BLOOM_INCREASE_RATE * Time.deltaTime; 
            prismMaterial.SetFloat("_Emission", currentBloom);
        }

        if(countdownToDestroy <= 2.0f && startCountdown)
        {
            // double bloom rate
            currentBloom += 2*BLOOM_INCREASE_RATE * Time.deltaTime;
            prismMaterial.SetFloat("_Emission", currentBloom);
        }

        if(countdownToDestroy <= 0.5f && startCountdown)
        {
            if(!destroyedList)
            {
                // destroy funky shader effects bc they mess with vfx
                for (int i = 0; i < effectsToDestroyBeforePlayingVFX.Length; ++i)
                {
                    if (effectsToDestroyBeforePlayingVFX[i] != null)
                    {
                        Destroy(effectsToDestroyBeforePlayingVFX[i]);
                    }
                }
                destroyedList = true;
            }

            if(!madeParticles)
            {
                if (destroyVFX != null)
                {
                    GameObject effect = new GameObject("VFX Instance");
                    effect.transform.position = prismGameObject.transform.position;
                    VisualEffect vfx = effect.AddComponent<VisualEffect>();
                    vfx.visualEffectAsset = destroyVFX;
                    vfx.Play();
                    //Instantiate(destroyVFX, prismGameObject.transform);
                }
                madeParticles = true;
            }
            
            // shrink object until it tiny
            Vector3 newScale = prismGameObject.transform.localScale - new Vector3(SHRINK_RATE, SHRINK_RATE, SHRINK_RATE);
            newScale = new Vector3(Mathf.Clamp01(newScale.x), Mathf.Clamp01(newScale.y), Mathf.Clamp01(newScale.z));
            prismGameObject.transform.localScale = newScale;
        }

        if(countdownToDestroy <= 0 && startCountdown)
        {
            Debug.Log("Final prism countdown ended!");
            // create vfx

            GameManager.Instance.WingDingTheGame();
            
            startCountdown=false;
        }
    }

}
