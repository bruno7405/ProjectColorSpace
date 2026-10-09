using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Helper script to play the dialog of Dialog manager
/// </summary>
public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] bool playOnStart;
    [SerializeField] Dialogue dialog;
    [SerializeField] bool playOnce = false;
    bool _hasPlayed = false;

    private void Start()
    {
        if (playOnStart)
        {
            StartCoroutine(PlayDialogDelay(1.0f));
        }
    }

    public void PlayDialog()
    {
        if (playOnce && _hasPlayed) return;
        
        _hasPlayed = true;
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
