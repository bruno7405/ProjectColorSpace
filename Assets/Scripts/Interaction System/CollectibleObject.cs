using UnityEngine;
using UnityEngine.Events;

public class CollectibleObject : MonoBehaviour, IInteractable
{
    [SerializeField] UnityEvent OnInteractEnter;
    [SerializeField] UnityEvent OnHoverEnter;
    [SerializeField] UnityEvent OnHoverExit;


    public void InteractEnter()
    {
        OnInteractEnter.Invoke();
    }

    public void HoverExit()
    {
        OnHoverExit.Invoke();
    }

    public void HoverEnter()
    {
        OnHoverEnter.Invoke();
    }
}