using UnityEngine;
using UnityEngine.Events;

public class VaultDoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] UnityEvent OnInteractEnter;
    [SerializeField] UnityEvent OnHoverEnter;
    [SerializeField] UnityEvent OnHoverExit;
    [SerializeField] VaultDoorAnimation anim;

    [SerializeField] string interactText;


    public void InteractEnter()
    {
        OnInteractEnter.Invoke();
        anim.TurnHandle();
    }

    public void HoverExit()
    {
        OnHoverExit.Invoke();
    }

    public void HoverEnter()
    {
        OnHoverEnter.Invoke();
    }

    public string GetInteractText()
    {
        return interactText;
    }
}