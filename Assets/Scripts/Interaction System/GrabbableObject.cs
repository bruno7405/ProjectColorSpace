using UnityEngine;
using UnityEngine.Events;

public class GrabbableObject : MonoBehaviour
{
    [SerializeField] UnityEvent OnGrabbed;
    [SerializeField] UnityEvent OnDropped;
    [SerializeField] UnityEvent OnHoverEnter;
    [SerializeField] UnityEvent OnHoverExit;

    private SpectrumObject _spectrumObject;
    private int _grabbableLayer;
    private int _heldLayer;
    private Rigidbody _rb;
    private bool _isGrabbed;

    protected virtual void Awake()
    {
        _spectrumObject = GetComponent<SpectrumObject>();
        _rb = GetComponent<Rigidbody>();
        _grabbableLayer = LayerMask.NameToLayer("Grabbable");
        _heldLayer = LayerMask.NameToLayer("HeldObject");
    }

    public void Grabbed()
    {
        OnGrabbed.Invoke();
        _isGrabbed = true;
        gameObject.layer = _heldLayer;

        if (_rb != null) _rb.isKinematic = true;
    }

    public void Dropped()
    {
        OnDropped.Invoke();
        _isGrabbed = false; // must be false before refreshing kinematic state
        gameObject.layer = _grabbableLayer;

        if (_spectrumObject != null) _spectrumObject.RefreshKinematic();
        else if (_rb != null) _rb.isKinematic = false;
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