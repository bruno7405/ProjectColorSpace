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
        // Hovering over valid interactable object
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, interactDistance, interactLayer)
            && hit.transform.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            if (currentInteractable != null && currentInteractable.Equals(interactable)) return;

            currentInteractable?.HoverExit();
            currentInteractable = interactable;
            currentInteractable.HoverEnter();
            OnInteractHoverEntered?.Invoke(currentInteractable.GetInteractText());
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
        currentInteractable?.InteractEnter();
        OnInteractEnter?.Invoke();
    }

    private void OnDrawGizmos()
    {
        if (cam == null) return;

        Gizmos.color = currentInteractable != null ? Color.green : Color.yellow;
        Gizmos.DrawRay(cam.position, cam.forward * interactDistance);
    }
}