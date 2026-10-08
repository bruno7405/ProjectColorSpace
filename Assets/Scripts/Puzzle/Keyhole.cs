using UnityEngine;
using UnityEngine.Events;

public class Keyhole : MonoBehaviour
{
    [SerializeField] private Transform attachPoint;
    [SerializeField] private SpectrumColor _requiredColor;

    [SerializeField] private UnityEvent OnKeyEnter;
    [SerializeField] private UnityEvent OnKeyExit;

    private Key _currentKey;

    public AudioClip keyedAudio;

    public bool HasKey => _currentKey != null;

    public bool TryAttachKey(Key key)
    {
        if (HasKey) return false;
        if (key.KeycColor != _requiredColor) return false;

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

        // Stop SpectrumObject from un-kinematic-ing the key while attached
        if (key.TryGetComponent(out SpectrumObject so))
        {
            so.ForceKinematic = true;
        }

        key.transform.SetPositionAndRotation(attachPoint.position, attachPoint.rotation);
        key.transform.SetParent(transform, true);

        _currentKey = key;
        key.SetKeyhole(this);
        OnKeyEnter.Invoke();
        AudioBus.Instance.PlaySFX(keyedAudio);
        return true;
    }

    public void DetachKey(Key key)
    {
        if (_currentKey != key) return;

        key.transform.SetParent(null, true);
        key.SetKeyhole(null);
        _currentKey = null;

        if (key.TryGetComponent(out SpectrumObject so))
        {
            so.ForceKinematic = false;
            so.RefreshKinematic();
        }

        OnKeyExit.Invoke();
    }

    public void DetachCurrentKey()
    {
        if (_currentKey != null) DetachKey(_currentKey);
    }
}