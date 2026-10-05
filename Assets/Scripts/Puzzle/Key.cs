using UnityEngine;

public class Key : GrabbableObject
{
    [SerializeField] private string keyId = "default";

    private Keyhole _currentKeyhole;

    public string KeyId => keyId;
    public bool IsAttached => _currentKeyhole != null;

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
}