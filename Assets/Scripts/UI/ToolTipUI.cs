using TMPro;
using UnityEngine;

public class ToolTipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI interactionTMP;

    private void Start()
    {
        // Interact
        PlayerInteractor.OnInteractHoverEntered += HandleInteractHoverEntered;
        PlayerInteractor.OnInteractHoverExited += HideInteractionText;
        PlayerInteractor.OnInteractEnter += HideInteractionText;

        // Grab
        PlayerGrabController.OnGrabHoverEntered += HandleGrabHoverEntered;
        PlayerGrabController.OnGrabHoverExited += HideInteractionText;
        PlayerGrabController.OnGrabbed += HandleGrabbed;
        PlayerGrabController.OnDropped += HideInteractionText;
    }

    private void OnDestroy()
    {
        // Interact
        PlayerInteractor.OnInteractHoverEntered -= HandleInteractHoverEntered;
        PlayerInteractor.OnInteractHoverExited -= HideInteractionText;
        PlayerInteractor.OnInteractEnter += HideInteractionText;

        // Grab
        PlayerGrabController.OnGrabHoverEntered -= HandleGrabHoverEntered;
        PlayerGrabController.OnGrabHoverExited -= HideInteractionText;
        PlayerGrabController.OnGrabbed -= HandleGrabbed;
        PlayerGrabController.OnDropped -= HideInteractionText;
    }

    private void HandleInteractHoverEntered(string interactText) => SetInteractionText(interactText);
    private void HandleGrabHoverEntered(string grabHoverText) => SetInteractionText(grabHoverText);
    private void HandleGrabbed(string grabbedText) => SetInteractionText(grabbedText);

    public void SetInteractionText(string text)
    {
        Debug.Log("Set Text: " + text);
        if (interactionTMP.text == text && interactionTMP.gameObject.activeInHierarchy) return;

        interactionTMP.gameObject.SetActive(true);
        interactionTMP.text = text;
    }

    public void HideInteractionText()
    {
        Debug.Log("Hide Text");
        interactionTMP.gameObject.SetActive(false);
        interactionTMP.text = "";
    }
}
