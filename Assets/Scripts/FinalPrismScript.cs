using UnityEngine;
using UnityEngine.VFX;

public class FinalPrismScript : MonoBehaviour
{
    [SerializeField] private GameObject prismGameObject;
    [SerializeField] private VisualEffect destroyVFX;
    [SerializeField] private DialoguePlayer dialoguePlayer;

    bool startCountdown = false;
    [SerializeField] private float countdownToDestroy = 5.00f; // wanna destroy right when/after he says i got roygbiv

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

        }
    }

}
