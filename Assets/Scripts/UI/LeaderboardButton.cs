using UnityEngine;

public class LeaderboardButton : MonoBehaviour
{
    
    void Awake()
    {
        GameSettings.OnSettingsChanged += HandleSettingsUpdate;
    }

    void Start()
    {
        HandleSettingsUpdate();
    }

    void OnDestroy()
    {
        GameSettings.OnSettingsChanged -= HandleSettingsUpdate;
    }

    void HandleSettingsUpdate()
    {
        gameObject.SetActive(GameSettings.SpeedrunMode);
    }
}
