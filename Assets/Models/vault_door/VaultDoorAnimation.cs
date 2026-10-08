using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

// this script's entire purpose is to make it easy for other scripts to open this door
// and also to fix the colliders once it opens :P
public class VaultDoorAnimation: MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] MeshCollider initialCollider;
    [SerializeField] MeshCollider finalCollider;
    bool opened = false;

    public void OpenDoor()
    {
        animator.Play("Open", 0, 0);
        finalCollider.enabled = true;
        initialCollider.enabled = false;
    }

    private void Start()
    {
        // disable initialCollider
        finalCollider.enabled = false;
    }

    void Update()
    {
         // for testing only
        if(Keyboard.current.zKey.isPressed && !opened)
        {
            OpenDoor();
            opened = true;
        }
    }
}
