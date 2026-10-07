using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmsScript : MonoBehaviour
{
    [SerializeField] private Transform watchBone;
    [SerializeField] private GameObject[] watchLockedColorBlockers = new GameObject[7];

    // this is red. 0 is like halfway between red & violet.
    private readonly float HUE_OFFSET = 0.07142857f;
    private Quaternion initialWatchBoneRotation;

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
    }

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
     * It will not move towards 0 until the player moves, but only while also NOT holding down a color shift input.
     * (so moving while shifting color continues to show the watch.)
     */

    float ANIMATION_SPEED = 0.75f;
    float positionInAnimation = 0f;
    bool shouldRaiseArm = false;

    void UpdateAnimation()
    {

    }

    void UpdateAnimationState()
    {
        if (Keyboard.current.qKey.isPressed || Keyboard.current.eKey.isPressed) // TODO may not be the proper input check
        {
            shouldRaiseArm = true;
        }
        else if(Keyboard.current.wKey.isPressed || 
            Keyboard.current.sKey.isPressed ||
            Keyboard.current.dKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.spaceKey.isPressed )
        {
            // in this branch, we already know color switch is NOT being pressed
            // AND we are moving, so lower arm
            shouldRaiseArm = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialWatchBoneRotation = watchBone.localRotation;

        // subscribe to hue update actions
        SpectrumManager.OnColorUpdate += OnHueChanged;
        SpectrumManager.OnColorUnlocked += OnColorUnlocked;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimationState();
        UpdateAnimation();
    }
}
