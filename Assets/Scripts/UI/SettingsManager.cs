using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public SettingsSlider MouseSense, Music, SFX;

    public void HandleCloseSettings()
    {
        gameObject.SetActive(false);
    }
}
