using UnityEngine;

public class ArmsScript : MonoBehaviour
{
    [SerializeField] private Transform watchBone;
    [SerializeField] private GameObject[] watchLockedColorBlockers = null;

    public void OnHueChanged(float h)
    {
        // get current world hue
        // update animation controller (??? todo)
        // set watchBone local Y rotation to match hue, where 0 degress is perfectly red
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
