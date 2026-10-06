using UnityEngine;

/*
    Testing out this funky idea, maybe we can define save data using a scriptable object in editor that we can load
    to make playtesting/debugging easier?
*/
[CreateAssetMenu(fileName = "SaveDataAsset", menuName = "Scriptable Objects/SaveDataAsset")]
public class SaveDataAsset : ScriptableObject
{
    public string uuid;
    // key flags
    public bool hasLibraryKey;
    public bool hasOfficeKey;
    public bool hasMasterBedroomKey;
    public bool hasMalloryBedroomKey;
    public bool hasSecretRoomKey;

    // item flags
    public bool HasMotherDeadNotice;
    public bool HasFatherJournal;
    public bool Hs1873Newspaper;
    public bool HasRosethroneLedger;
    public bool HsCreditorTelegram;
    public bool HasMallorysJournal;
    public bool HasTrainTicket;
    public bool HasPackedTrunk;
    public bool HasNewspaperDeaths;
    public bool HasMurderWeapon;
    public bool HasSuicideWeapon;
    public bool HasFathersNote;

    public SaveData ToSaveData()
    {
        return new SaveData
        {
            uuid = uuid
        };
        // expand as needed
    }
}
