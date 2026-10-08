using System.Collections;
using UnityEngine;

/// <summary>
/// Helper script to play the dialog of Dialog manager
/// </summary>
public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] bool playOnStart;
    [SerializeField] Dialogue dialog;

    private void Start()
    {
        if (playOnStart)
        {
            StartCoroutine(PlayDialogDelay(2.5f));
        }
    }

    public void PlayDialog()
    {
        StartCoroutine(PlayDialogDelay(2.5f));
    }

    IEnumerator PlayDialogDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DialogueSystem.Instance.PlayDialogue(dialog);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
