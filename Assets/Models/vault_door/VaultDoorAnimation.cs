using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

// this script's entire purpose is to make it easy for other scripts to open this door
// and also to fix the colliders once it opens :P
public class VaultDoorAnimation: MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject handleInteractBox;
    [SerializeField] MeshCollider initialCollider;
    [SerializeField] MeshCollider finalCollider;
    [SerializeField] DialoguePlayer dialoguePlayer;
    bool opened = false;
    bool shouldOpenSoon = false;

    public void TurnHandle()
    {
        // Debug.Log("turnhandle called");
        animator.Play("TurnHandle", 0, 0);
        shouldOpenSoon = true;
        handleInteractBox.SetActive(false);
    }

    void OpenDoor()
    {
        dialoguePlayer.PlayDialog();
        // Debug.Log("opendoor called");
        animator.Play("Open", 0, 0);
        finalCollider.enabled = true;
        initialCollider.enabled = false;
    }

    private void Start()
    {
        // disable finalCollider
        finalCollider.enabled = false;
    }

    void Update()
    {
         // for testing only
        if(Keyboard.current.zKey.wasPressedThisFrame && !opened)
        {
            TurnHandle();
            opened = true;
        }

        if(shouldOpenSoon)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            /*Debug.Log(
                $"State: {state.fullPathHash}, " +
                $"Time: {state.normalizedTime}, " +
                $"Transitioning: {animator.IsInTransition(0)}"
            );*/

            if (state.IsName("TurnHandle")
                && state.normalizedTime >= 1f
                && !animator.IsInTransition(0))
                {
                // Debug.Log("animator finished");
                OpenDoor();
                shouldOpenSoon=false;
            }
        }
        
    }
}
