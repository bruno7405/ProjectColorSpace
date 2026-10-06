using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    
    [Header("PLAYTESTING ONLY! LEAVE BLANK WHEN FINAL BUILD!!!")]
    public SaveDataAsset debugStartData;

    private string Root => Application.persistentDataPath + "/";
    public string SaveFileName = "save.data";

    public SaveData CurrentSave { get; private set; }

    private bool _debugMode = false;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Debug.LogWarning(">1 Save Manager in scene!");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (debugStartData != null)
        {
            CurrentSave = debugStartData.ToSaveData();
            _debugMode = true;
            Debug.Log("[SaveManager]: Loaded debug save data!");
        }  
        else
        {
            CurrentSave = LoadGame();
            if (CurrentSave == null)
            {
                Debug.Log("[SaveManager]: No save data found, using defaults!");
                CurrentSave = new SaveData
                {
                    // setup defaults here
                    uuid = System.Guid.NewGuid().ToString() 
                };
            } else
            {
                Debug.Log("[SaveManager]: Loaded save data!");
            }
        }

        SignalBus.Subscribe(SaveSignal.SaveGame, Save);
        SignalBus.Subscribe(SaveSignal.LoadGame, Load);
    } 

    void OnDestroy()
    {
        if (!_debugMode) SaveGame(CurrentSave); 
        // to not override our saved games later on
        // also this doesn't work with webgl i'm suddenly realizing, uhh

        // so there are ways around this that forces unity to save these files to the browser cache
        // im not doing this tonight though lmao, TODO HERE
    }

    private void Save() => SaveGame(CurrentSave);
    private void Load() => CurrentSave = LoadGame();

    public void SaveGame(SaveData data)
    {
       string path = Path.Combine(Root, SaveFileName);
       
       string json = JsonConvert.SerializeObject(data, Formatting.Indented);
       File.WriteAllText(path, json);

       Debug.Log("[SaveManager]: Saved game to " + path);
    }

    public SaveData LoadGame()
    {
        string path = Path.Combine(Root, SaveFileName);
        if (!File.Exists(path)) return null;

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<SaveData>(json);
    }
}

public enum SaveSignal
{
    SaveGame,
    LoadGame
}
