using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string uuid; // just for testing if this works how I think it might

    /*
    Some thoughts on how this can expand.
    Log the player's last position from a save state?
    Ghost anger and other things?

    Objects/story progressions can just be a set of flags, such as

    HasTrinket = true or false or whatever

    Like Deltarune! But at least our flags have names lol
    */

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

    /*
    Mother's death notice → Mallory's mother died giving birth to her.
    Father's old journal → he blamed Mallory from childhood.
    1873 newspaper → explains the financial crash.
    Rosethorne ledger + creditor telegram → proves her father was financially ruined.
    Mallory's journal → reveals his behavior became increasingly frightening.
    Train ticket + packed trunk/money → Mallory intended to flee.
    Newspaper covering the deaths → gives James the official family story.
    This would be in servant's rooms
    Office/Study key → finally opens the sealed crime scene.
    Murder weapon → establishes she was killed there.
    Suicide weapon + father's final note → establishes what happened afterward.
    */
}
