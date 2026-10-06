using UnityEngine;

/*
    Testing out this funky idea, maybe we can define save data using a scriptable object in editor that we can load
    to make playtesting/debugging easier?
*/
[CreateAssetMenu(fileName = "SaveDataAsset", menuName = "Scriptable Objects/SaveDataAsset")]
public class SaveDataAsset : ScriptableObject
{
    public string uuid;

    public SaveData ToSaveData()
    {
        return new SaveData
        {
            uuid = uuid
        };
        // expand as needed
    }
}
