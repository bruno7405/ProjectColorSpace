using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI interactionTMP;

    private void Start()
    {
        PlayerGrabController.OnObjectHoverEntered += () => { SetInteractionText("[F] Pickup"); };
        PlayerGrabController.OnObjectHoverExited += () => HideInteractionText();
        PlayerGrabController.OnObjectGrabbed += () => { SetInteractionText("[F] Drop"); };
        PlayerGrabController.OnObjectDropped += () => HideInteractionText();
    }

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
