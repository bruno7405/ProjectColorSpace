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

    private void Awake()
    {
        _spectrumObject = GetComponent<SpectrumObject>();
        _rb = GetComponent<Rigidbody>();
        _grabbableLayer = LayerMask.NameToLayer("Grabbable");
        _heldLayer = LayerMask.NameToLayer("HeldObject");
    }

    public void Grabbed()
    {
        OnGrabbed.Invoke();
        _rb.isKinematic = true;
        gameObject.layer = _heldLayer;
        _isGrabbed = true;
    }

    public void Dropped()
    {
        OnDropped.Invoke();
        _rb.isKinematic = _spectrumObject != null && !_spectrumObject.IsSolid;
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