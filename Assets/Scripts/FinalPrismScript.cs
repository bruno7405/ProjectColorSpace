using UnityEngine;
using UnityEngine.VFX;

public class FinalPrismScript : MonoBehaviour
{
    [SerializeField] private GameObject prismGameObject;
    [SerializeField] private VisualEffect destroyVFX;

    bool startCountdown = false;
    [SerializeField] private float countdownToDestroy = 5.00f; // wanna destroy right when/after he says i got roygbiv

    public void StartDestroyPrism()
    {
        startCountdown = true;
        Debug.Log("Start destroy prism");
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

     void Update()
    {
        if(startCountdown)
        {
            countdownToDestroy -= Time.deltaTime;
        }

        if(countdownToDestroy <= 0)
        {
            if(destroyVFX != null)
            {
                Instantiate(destroyVFX, prismGameObject.transform);
            }
        }
    }

}
