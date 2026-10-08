using UnityEngine;

public class WorldBorder : MonoBehaviour
{
    [SerializeField] Transform teleportTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        var cc = other.GetComponent<CharacterController>();

        if (cc != null) cc.enabled = false;

        other.transform.SetPositionAndRotation(teleportTransform.position, teleportTransform.rotation);

        if (cc != null) cc.enabled = true;
    }
}