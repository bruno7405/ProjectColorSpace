using UnityEngine;

public class LeftArmScript: MonoBehaviour
{ 
    // couldve made one script to reuse between arms but eh dont feel like it
    [SerializeField] private Animator armAnimator;
    [SerializeField] private readonly float ANIMATION_SPEED_UP = 0.080f; // quadrupled in develop branch
    [SerializeField] private readonly float ANIMATION_SPEED_DOWN = 0.020f; // quadrupled in develop branch
    float positionInAnimation = 0f;
    float currTimeout = 0;
    bool shouldRaiseArm = false;
    bool holdingObject = false;

    void UpdateAnimation()
    {
        Debug.Log(positionInAnimation);
        if (shouldRaiseArm)
        {
            positionInAnimation += ANIMATION_SPEED_UP;
        }
        else
        {
            positionInAnimation -= ANIMATION_SPEED_DOWN;
        }
        positionInAnimation = Mathf.Clamp01(positionInAnimation);

        armAnimator.Play("HoldArmOutToGrab", 0, positionInAnimation);
    }

    void UpdateAnimationState()
    {
        // TODO check if holding an object
        if(!holdingObject)
        {
            if(currTimeout <= 0)
            {
                shouldRaiseArm = false;
            }
        }
        else
        {
            shouldRaiseArm = true;
        }
    }

    void OnObjectGrabbed(string str)
    {
        holdingObject = true;
    }

    void OnObjectDropped()
    {
        holdingObject = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerGrabController.OnGrabbed += OnObjectGrabbed;
        PlayerGrabController.OnDropped += OnObjectDropped;
        armAnimator.Play("HoldArmOutToGrab", 0, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        currTimeout -= Time.deltaTime;
        UpdateAnimationState();
        UpdateAnimation();
    }
}
