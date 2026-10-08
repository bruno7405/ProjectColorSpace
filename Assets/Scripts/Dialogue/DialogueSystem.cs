using System.Collections;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] DialogUI dialogUI;
    [SerializeField] Dialogue redDialogue;
    Coroutine currentRoutine;

    public bool IsPlaying => currentRoutine != null;


    private void Start()
    {
        PlayDialogue(redDialogue);
    }

    public void PlayDialogue(Dialogue dialogue)
    {
        if (dialogue == null) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PlayRoutine(dialogue));
    }

    IEnumerator PlayRoutine(Dialogue dialogue)
    {
        foreach (var line in dialogue.GetLines())
        {
            dialogUI.SetDialog(line.dialogLine);

            if (line.dialogAudio != null)
            {
                AudioBus.Instance.PlaySFX(line.dialogAudio, 2, false);
                yield return new WaitForSeconds(line.dialogAudio.length);
            }
            else
            {
                yield return new WaitForSeconds(3);
            }
        }
        dialogUI.ClearDialog();
        currentRoutine = null;
    }
}