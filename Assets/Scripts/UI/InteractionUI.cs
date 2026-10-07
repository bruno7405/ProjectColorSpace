using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI interactionTMP;

    private void Start()
    {
        PlayerGrabController.OnObjectHoverEntered += HandleHoverEntered;
        PlayerGrabController.OnObjectHoverExited += HandleHoverExited;
        PlayerGrabController.OnObjectGrabbed += HandleGrabbed;
        PlayerGrabController.OnObjectDropped += HandleDropped;
    }

    private void OnDestroy()
    {
        PlayerGrabController.OnObjectHoverEntered -= HandleHoverEntered;
        PlayerGrabController.OnObjectHoverExited -= HandleHoverExited;
        PlayerGrabController.OnObjectGrabbed -= HandleGrabbed;
        PlayerGrabController.OnObjectDropped -= HandleDropped;
    }

    private void HandleHoverEntered() => SetInteractionText("[F] Pickup");
    private void HandleHoverExited() => HideInteractionText();
    private void HandleGrabbed() => SetInteractionText("[F] Drop");
    private void HandleDropped() => HideInteractionText();

    public void SetInteractionText(string text)
    {
        if (interactionTMP.text == text && interactionTMP.gameObject.activeInHierarchy) return;

        interactionTMP.gameObject.SetActive(true);
        interactionTMP.text = text;
    }

    public void HideInteractionText()
    {
        interactionTMP.gameObject.SetActive(false);
        interactionTMP.text = "";
    }
}
