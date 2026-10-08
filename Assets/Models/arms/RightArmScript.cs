using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmsScript : MonoBehaviour
{
    [SerializeField] private Transform watchBone;
    [SerializeField] private GameObject[] watchLockedColorBlockers = new GameObject[7];

    [SerializeField] private Animator armAnimator;
    [SerializeField] private readonly float ANIMATION_SPEED_UP = 0.080f; // quadrupled in develop branch
    [SerializeField] private readonly float ANIMATION_SPEED_DOWN = 0.020f; // quadrupled in develop branch
    [SerializeField] private readonly float TIMEOUT_SECONDS = 1.200f; // delta time is in seconds :(
    [SerializeField] private readonly float TIMEOUT_WHILE_MOVING_SECONDS = 0.800f;

    // this is red. 0 is like halfway between red & violet.
    private readonly float HUE_OFFSET = 0.07142857f;
    private Quaternion initialWatchBoneRotation;

    float positionInAnimation = 0f;
    float currTimeout = 0;
    bool shouldRaiseArm = false;
    bool isMoving = false;

    public void OnHueChanged(float h)
    {
        //Debug.Log("hue " + h);
        // current world hue should be h. it goes between 0 and 1.
        // shift such that "perfectly red" = 0
        // ok also the watch needle starts in the middle of red so it needs to double
        float shiftedHue = h - 2*HUE_OFFSET;
        shiftedHue = Mathf.Repeat(shiftedHue, 1f); //wraparound

        // set watchBone local rotation to match hue
        // i hate quaternion
        float rotation = 360 - (shiftedHue * 360f); // WHY DO IT GO BACKWARDS
        watchBone.localRotation = initialWatchBoneRotation * Quaternion.Euler(0f, rotation, 0f);

        if(!shouldRaiseArm) { Debug.Log("set raisearm to true"); }
        shouldRaiseArm = true;
        if (isMoving) { currTimeout = TIMEOUT_WHILE_MOVING_SECONDS; } else { currTimeout = TIMEOUT_SECONDS; };
    }

    // TODO test this
    public void OnColorUnlocked(int index)
    {
        Debug.Log("unlocked " + index);
        // player has red unlocked from the start
        // unlocking orange triggers actual watch functionality. with just red, it dont move
        // orange should be 2nd, at index 1. our blockers, start at index 0.
        int shiftedIndex = index - 1;
        // remove color blocker, if it still exists
        if (shiftedIndex < 0 || shiftedIndex >= watchLockedColorBlockers.Length) return;
        if (watchLockedColorBlockers[shiftedIndex] != null)
        {
            Destroy(watchLockedColorBlockers[shiftedIndex]);
            watchLockedColorBlockers[shiftedIndex] = null;

            // unlocking violet removes final 2 blockers instead of just 1
            if(index == 6 && watchLockedColorBlockers[index] != null)
            {
                Destroy(watchLockedColorBlockers[index]);
                watchLockedColorBlockers[index] = null;
            }
        }
    }

    // =============================================================================================================================================

    /*
     * I am going to treat the arm-up animation like a pose slider from 0 to 1.
     * 0 is arm is completely down, 1 is arm is completely up.
     * When the player inputs a color shift, it will start moving towards 1, and stay there if it reaches it.
     * It will not move towards 0 until after a short timeout & the player moves.
     */

    void UpdateAnimation()
    {
        Debug.Log(positionInAnimation);
        if(shouldRaiseArm)
        {
            positionInAnimation += ANIMATION_SPEED_UP;
        } else
        {
            positionInAnimation -= ANIMATION_SPEED_DOWN;
        }
        positionInAnimation = Mathf.Clamp01(positionInAnimation);

        armAnimator.Play("ArmUp", 0, positionInAnimation);
    }

    /* TROUBLESHOOTING
     * - shouldRaiseArm confirmed to be set to true/false when expected
     * - setting speed back to 1 doesnt fix anything, just makes it behave like speed is set to 1
     * - using Play instead of Update seems to work
     * - but raising the arm is really inconsistent, whereas lowering it works perfectly
     * - removing the has-changed check fixed this. now pausing seems to always raise the arm. weird but not high priority
     * 
     * - since splitting arms & adding to player prefab, right arm raising has become inconsistent
     * - removing moving requirement to lower has not fixed it (idk why it would)
     * - 
     */

    void UpdateAnimationState()
    {
        if(Keyboard.current.wKey.isPressed || 
            Keyboard.current.sKey.isPressed ||
            Keyboard.current.dKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.spaceKey.isPressed )
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (currTimeout <= 0)
        {
            shouldRaiseArm = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialWatchBoneRotation = watchBone.localRotation;
        armAnimator.Play("ArmUp", 0, 0f);

        // subscribe to hue update actions
        SpectrumManager.OnColorUpdate += OnHueChanged;
        SpectrumManager.OnColorUnlocked += OnColorUnlocked;
    }

    // Update is called once per frame
    void Update()
    {
        currTimeout -= Time.deltaTime;
        UpdateAnimationState();
        UpdateAnimation();
    }
}
