using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] float interactDistance;
    [SerializeField] LayerMask interactLayer;

    public static Action<string> OnInteractHoverEntered;
    public static Action OnInteractHoverExited;
    public static Action OnInteractEnter;

    private IInteractable currentInteractable;
    private Transform cam;

    public AudioClip pickupAudio;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {

        CheckForInteractable();

        if (Keyboard.current.fKey.isPressed)
        {
            Interact();
        }
    }

    private void CheckForInteractable()
    {
        if (PlayerGrabController.Instance.IsHolding || PlayerGrabController.Instance.IsHoveringGrabbable) return;
        // Hovering over valid interactable object
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, interactDistance, interactLayer)
            && hit.transform.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            if (currentInteractable != null && currentInteractable.Equals(interactable)) return;

            currentInteractable?.HoverExit();
            currentInteractable = interactable;
            currentInteractable.HoverEnter();
            OnInteractHoverEntered?.Invoke("[F] " + currentInteractable.GetInteractText());
            return;
        }

        if (currentInteractable == null) return;
        // Not hovering on any interactable
        currentInteractable?.HoverExit();
        currentInteractable = null;
        OnInteractHoverExited?.Invoke();
    }

    private void Interact()
    {
        if (currentInteractable == null) return;
        currentInteractable?.InteractEnter();
        AudioBus.Instance.PlaySFX(pickupAudio);
        OnInteractEnter?.Invoke();
    }

    private void OnDrawGizmos()
    {
        if (cam == null) return;

        Gizmos.color = currentInteractable != null ? Color.green : Color.yellow;
        Gizmos.DrawRay(cam.position, cam.forward * interactDistance);
    }
}