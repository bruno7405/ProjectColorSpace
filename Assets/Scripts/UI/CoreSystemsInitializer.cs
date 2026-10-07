using UnityEngine;

public class CoreSystemsInitializer : MonoBehaviour
{
    public static CoreSystemsInitializer Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(this);
    }
}
