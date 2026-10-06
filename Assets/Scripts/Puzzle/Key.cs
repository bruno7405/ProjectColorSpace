using UnityEngine;

[RequireComponent(typeof(SpectrumObject))]
public class Key : GrabbableObject
{
    [SerializeField] private SpectrumColor _keyColor;
    private SpectrumObject _spectrumObject;
    private Collider _collider;
    private Keyhole _currentKeyhole;

    public SpectrumColor KeycColor => _keyColor;
    public bool IsAttached => _currentKeyhole != null;

    protected override void Awake()
    {
        base.Awake();
        _spectrumObject = GetComponent<SpectrumObject>();
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _spectrumObject.OnSolidChanged += UpdateAttachState;
    }

    private void OnDestroy()
    {
        if (_spectrumObject != null) _spectrumObject.OnSolidChanged -= UpdateAttachState;
    }

    // Attach only once the key is released inside the keyhole's trigger
    private void OnTriggerStay(Collider other)
    {
        if (IsAttached || IsGrabbed()) return;

        Keyhole keyhole = other.GetComponentInParent<Keyhole>();
        if (keyhole != null)
        {
            keyhole.TryAttachKey(this);
        }
    }

    // If the player grabs the key again, pull it out of the keyhole
    private void Update()
    {
        if (IsAttached && IsGrabbed())
        {
            _currentKeyhole.DetachKey(this);
        }
    }

    public void SetKeyhole(Keyhole keyhole)
    {
        _currentKeyhole = keyhole;
    }

    private void UpdateAttachState(bool isSolid)
    {
        if (!isSolid && _currentKeyhole != null)
        {
            _currentKeyhole.DetachKey(this);
        }
    }
}