using UnityEngine;
using UnityEngine.Events;

public class GrabbableObject : MonoBehaviour
{
    [SerializeField] UnityEvent OnGrabbed;
    [SerializeField] UnityEvent OnDropped;
    [SerializeField] UnityEvent OnHoverEnter;
    [SerializeField] UnityEvent OnHoverExit;

    protected string grabbableObjectName;

    private SpectrumObject _spectrumObject;
    private int _grabbableLayer;
    private int _heldLayer;
    private Rigidbody _rb;
    private bool _isGrabbed;

    float regrabCooldown = 0.5f;

    private float _nextGrabTime;

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

    public bool CanBeGrabbed()
    {
        return !_isGrabbed && Time.time >= _nextGrabTime;
    }

    public void Dropped()
    {
        OnDropped.Invoke();
        _isGrabbed = false; // must be false before refreshing kinematic state
        gameObject.layer = _grabbableLayer;
        _nextGrabTime = Time.time + regrabCooldown;

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

    public string GetName()
    {
        return grabbableObjectName;
    }
}