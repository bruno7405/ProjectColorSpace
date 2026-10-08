using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject doorModel;

    [SerializeField] Transform doorTransform;
    [SerializeField] float doorSpeed = 10;
    [SerializeField] Transform openTransform;

    AudioSource audioSource; //needs to be a 3D sound
    [SerializeField] AudioClip doorOpenSFX;

    Vector3 closedPosition;
    Vector3 openPosition;
    private Vector3 target;
    Coroutine moveRoutine;

    private Vector3 lastPosition;
    public Vector3 CalculatedVelocity { get; private set; }

    private bool isOpen = false;

    private void Awake()
    {
        closedPosition = doorTransform.position;
        openPosition = openTransform.position;
        target = closedPosition;

        audioSource = GetComponent<AudioSource>();
    }

    public void Open()
    {
        isOpen = true;
        target = openPosition;
        audioSource.volume = GameSettings.SFXVolume;
        audioSource.PlayOneShot(doorOpenSFX);
    }

    public void Close()
    {
        isOpen = false;
        target = closedPosition;
        audioSource.volume = GameSettings.SFXVolume;
        audioSource.PlayOneShot(doorOpenSFX);
    }

    public void FixedUpdate()
    {
        if ((doorTransform.position - target).magnitude > 0.001f)
        {
            doorTransform.position = Vector3.MoveTowards(
                doorTransform.position, target, doorSpeed * Time.deltaTime);
        }

        Vector3 displacement = transform.position - lastPosition;
        CalculatedVelocity = displacement / Time.fixedDeltaTime;
        lastPosition = transform.position;

    }
}