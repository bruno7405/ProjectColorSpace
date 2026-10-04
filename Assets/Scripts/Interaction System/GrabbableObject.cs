using UnityEngine;
using UnityEngine.Events;

public class GrabbableObject : MonoBehaviour
{
    [SerializeField] UnityEvent OnGrabEnter;
    [SerializeField] UnityEvent OnGrabExit;
    [SerializeField] UnityEvent OnHoverEnter;
    [SerializeField] UnityEvent OnHoverExit;

    private int _grabbableLayer;
    private int _heldLayer;

    private Rigidbody _rb;
    private bool _isGrabbed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _grabbableLayer = LayerMask.NameToLayer("Grabbable");
        _heldLayer = LayerMask.NameToLayer("HeldObject");
    }

    public void Grabbed()
    {
        OnGrabEnter.Invoke();
        _rb.isKinematic = true;
        gameObject.layer = _heldLayer;
        _isGrabbed = true;
    }

    public void Dropped()
    {
        OnGrabEnter.Invoke();
        _rb.isKinematic = false;
        gameObject.layer = _grabbableLayer;
        _isGrabbed = false;
    }

    public void HoverExit()
    {
        OnHoverExit.Invoke();
    }

    public void HoverEnter()
    {
        OnHoverEnter.Invoke();
    }

    public bool IsGrabbed()
    {
        return _isGrabbed;
    }
}