using System;
using UnityEngine;

[Serializable]
public struct Dialogue
{
    [TextArea(3, 5)]
    public string dialogLine;
    public AudioClip dialogAudio;
}
