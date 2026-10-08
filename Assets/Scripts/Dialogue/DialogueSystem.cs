using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] DialogUI dialogUI;
    [SerializeField] Dialogue redDialogue;

    Coroutine currentRoutine;
    AudioSource currentVoice;

    public static DialogueSystem Instance { get; private set; }
    public bool IsPlaying => currentRoutine != null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(transform.root.gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance != this) return;
        if (mode != LoadSceneMode.Single) return;    
        StopDialogue();
    }

    public void PlayDialogue(Dialogue dialogue)
    {
        if (dialogue == null) return;

        StopDialogue(clearUI: false);
        currentRoutine = StartCoroutine(PlayRoutine(dialogue));
    }

    public void StopDialogue(bool clearUI = true)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        if (currentVoice != null)
        {
            Destroy(currentVoice.gameObject);
            currentVoice = null;
        }

        if (clearUI && dialogUI != null)
            dialogUI.ClearDialog();
    }

    IEnumerator PlayRoutine(Dialogue dialogue)
    {
        foreach (var line in dialogue.GetLines())
        {
            if (dialogUI == null) break;
            dialogUI.SetDialog(line.dialogLine);

            if (line.dialogAudio != null)
            {
                currentVoice = AudioBus.Instance.PlaySFX(line.dialogAudio, 2, false, false);

                yield return new WaitForSeconds(line.dialogAudio.length);
            }
            else
            {
                yield return new WaitForSeconds(3);
            }
        }

        currentVoice = null;
        currentRoutine = null;
        if (dialogUI != null) dialogUI.ClearDialog();
    }
}