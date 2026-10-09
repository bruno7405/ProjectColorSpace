using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(200)]
public class Door : MonoBehaviour, IMovingSurface
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

    private Vector3 lastPosition;
    public Vector3 DeltaMovement { get; private set; }

    private bool isOpen = false;

    private void Awake()
    {
        closedPosition = doorTransform.position;
        openPosition = openTransform.position;
        target = closedPosition;
        lastPosition = doorTransform.position;

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

    public void Update()
    {
        if ((doorTransform.position - target).sqrMagnitude > 0.000001f)
        {
            doorTransform.position = Vector3.MoveTowards(
                doorTransform.position, target, doorSpeed * Time.deltaTime);
        }

        DeltaMovement = doorTransform.position - lastPosition;
        lastPosition = doorTransform.position;
    }
}