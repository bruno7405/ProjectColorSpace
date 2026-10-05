using UnityEngine;
using UnityEngine.Events;

public class Keyhole : MonoBehaviour
{
    [SerializeField] private Transform attachPoint;
    [SerializeField] private string requiredKeyId = "default";

    [SerializeField] private UnityEvent OnKeyEnter;
    [SerializeField] private UnityEvent OnKeyExit;

    private Key _currentKey;

    public bool HasKey => _currentKey != null;

    public bool TryAttachKey(Key key)
    {
        if (HasKey) return false;
        if (key.KeyId != requiredKeyId) return false;

        // Stop any physics motion, then lock the key in place
        if (key.TryGetComponent(out Rigidbody rb))
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = true;
        }

        key.transform.SetPositionAndRotation(attachPoint.position, attachPoint.rotation);
        key.transform.SetParent(transform, true);

        _currentKey = key;
        key.SetKeyhole(this);
        OnKeyEnter.Invoke();
        return true;
    }

    public void DetachKey(Key key)
    {
        if (_currentKey != key) return;

        key.transform.SetParent(null, true);
        key.SetKeyhole(null);
        _currentKey = null;
        OnKeyExit.Invoke();
    }

    public void DetachCurrentKey()
    {
        DetachKey(_currentKey);
    }
}