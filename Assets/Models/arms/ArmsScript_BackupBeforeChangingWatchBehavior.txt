using System;
using UnityEngine;

public class ArmsScript : MonoBehaviour
{
    [SerializeField] private Transform watchBone;
    [SerializeField] private GameObject[] watchLockedColorBlockers = new GameObject[5];

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

        // update animation controller (??? todo whether this is needed)

        // set watchBone local rotation to match hue
        // i hate quaternion
        float rotation = 360 - (shiftedHue * 360f); // WHY DO IT GO BACKWARDS
        watchBone.localRotation = initialWatchBoneRotation * Quaternion.Euler(0f, rotation, 0f);
    }

    public void OnColorUnlocked(int index)
    {
        Debug.Log("unlocked " + index);
        // player has red & orange unlocked from the start
        int shiftedIndex = index - 2;
        // remove color blocker, if it still exists
        if (shiftedIndex < 0 || shiftedIndex >= watchLockedColorBlockers.Length) return;
        if (watchLockedColorBlockers[shiftedIndex] != null)
        {
            Destroy(watchLockedColorBlockers[shiftedIndex]);
            watchLockedColorBlockers[shiftedIndex] = null;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialWatchBoneRotation = watchBone.localRotation;

        // subscribe to hue update action
        SpectrumManager.OnColorUpdate += OnHueChanged;
        SpectrumManager.OnColorUnlocked += OnColorUnlocked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
