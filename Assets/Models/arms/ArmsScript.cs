using System;
using UnityEngine;

public class ArmsScript : MonoBehaviour
{
    [SerializeField] private Transform watchBone;
    [SerializeField] private GameObject[] watchLockedColorBlockers = null;

    // this is red. 0 is like halfway between red & violet.
    private readonly float HUE_OFFSET = 0.07142857f;

    public void OnHueChanged(float h)
    {
        Debug.Log("hue " + h);
        // current world hue should be h. it goes between 0 and 1.
        // shift such that "perfectly red" = 0
        // ok also the watch needle starts in the middle of red so it needs to double
        float shiftedHue = h - 2*HUE_OFFSET;
        shiftedHue = Mathf.Repeat(shiftedHue, 1f); //wraparound

        // update animation controller (??? todo whether this is needed)

        // set watchBone local rotation to match hue
        float rotation = shiftedHue * 360f;
        Vector3 euler = watchBone.localEulerAngles;
        Debug.Log("angles before rotation: " + euler);
        euler.x = rotation; // idk why it is x. this makes no sense to me
        watchBone.localEulerAngles = euler;
        Debug.Log("after rotation: " + euler);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // subscribe to hue update action
        SpectrumManager.OnColorUpdate += OnHueChanged; // idk how to do this
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
