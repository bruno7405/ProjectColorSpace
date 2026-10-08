using UnityEngine;

public class MusicLayerInit : MonoBehaviour
{
    public AudioClip MainLoopMusic;
    public AudioClip[] LoopMusicLayers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        AudioBus.Instance.PlayMusic(MainLoopMusic);
        foreach (AudioClip layer in LoopMusicLayers) {
            AudioBus.Instance.InitMusicLayer(layer);
        }

        AudioBus.Instance.SubscribeToLayerScroll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
