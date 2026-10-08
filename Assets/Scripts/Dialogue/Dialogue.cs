using System;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Serializable]
    public struct DialogueLine
    {
        [TextArea(3, 5)]
        public string dialogLine;
        public AudioClip dialogAudio;

        public float clipLength;
    }

    [SerializeField] List<DialogueLine> dialogLines = new List<DialogueLine>();

    public List<DialogueLine> GetLines()
    {
        return dialogLines;
    }
}
