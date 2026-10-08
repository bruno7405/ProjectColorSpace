using UnityEngine;
using System.Collections.Generic;

public class AudioBus : MonoBehaviour
{
    public static AudioBus Instance;
    const float defaultMusicVolume = 0.5f;
    const float defaultSFXVolume = 1.0f;
    public float pitchRandom = 0.15f;

    // UI Button SFX
    public static AudioClip DefaultButtonHoverSFX { get; private set; }
    public static AudioClip DefaultButtonClickSFX { get; private set; }

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DefaultButtonHoverSFX = Resources.Load<AudioClip>("ButtonHover");
        DefaultButtonClickSFX = Resources.Load<AudioClip>("ButtonClick");
    }
    
    // Audio source references
    private AudioSource musicSource;
    private List<AudioSource> musicSourceLayers = new List<AudioSource>();
    private AudioSource sfxSource;
    
    void Start()
    {
        // Create and configure audio sources
        SetupAudioSources();
    }

    public void SubscribeToLayerScroll()
    {
        SpectrumManager.OnColorUpdate += LayerScroll;
    }
    
    void SetupAudioSources()
    {
        // Create two audio sources - one for music, one for SFX
        
        // Music audio source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;        // Music usually loops
        musicSource.volume = defaultMusicVolume;      // Lower volume for background music
        musicSource.priority = 0;       // Highest priority
        
        // SFX audio source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;         // Sound effects don't usually loop
        sfxSource.volume = defaultSFXVolume;        // Full volume for sound effects
        sfxSource.priority = 128;       // Normal priority
    }
    
    // Play a music track
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        
        // Stop any currently playing music
        musicSource.Stop();
        
        // Set the new music clip
        musicSource.clip = clip;
        musicSource.loop = loop;
        
        // Start playing
        musicSource.Play();
    }
    
    // Play a sound effect with volume control
    public void PlaySFX(AudioClip clip, float volume = 1.0f, bool pitchRandomization = true)
    {
        if (clip == null) return;

        GameObject soundSource = new GameObject($"SFX_{clip.name}");
        soundSource.transform.position = transform.position;
        soundSource.transform.SetParent(transform);

        AudioSource source = soundSource.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.loop = false;
        if (pitchRandomization) {
            source.pitch = Random.Range(1.0f - pitchRandom, 1.0f + pitchRandom);
        } else {
            source.pitch = 1.0f;
        }
        source.Play();
        // Higher pitch makes the clip finish sooner.
        Destroy(soundSource, clip.length / source.pitch);
    }

    public void PlayOneshotSFX(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip, volume);
    }
    
    public void PlayRandomSFX(AudioClip[] clips) {
        PlaySFX(clips[Random.Range(0, clips.Length)]);
    }
    // Control music volume
    public void SetMusicVolume(float volume = defaultMusicVolume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }
    
    // Control SFX volume
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
    
    // Fade music in
    public void FadeMusicIn(float duration = 1.0f)
    {
        StartCoroutine(FadeMusicVolume(0, musicSource.volume, duration));
    }
    
    // Fade music out
    public void FadeMusicOut(float duration = 1.0f)
    {
        StartCoroutine(FadeMusicVolume(musicSource.volume, 0, duration));
    }
    
    // Coroutine to fade music volume
    private System.Collections.IEnumerator FadeMusicVolume(float startVolume, float targetVolume, float duration)
    {
        float startTime = Time.time;
        float elapsedTime = 0;
        
        musicSource.volume = startVolume;
        
        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            float normalizedTime = elapsedTime / duration;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, normalizedTime);
            yield return null;
        }
        
        musicSource.volume = targetVolume;
    }

    ///
    /// MUSIC LAYERS
    /// 
    
    public void InitMusicLayer(AudioClip layer) {
        GameObject layerObject = new GameObject(layer.name);
        AudioSource layerSource = layerObject.AddComponent<AudioSource>();
        layerObject.transform.parent = transform;
        layerSource.clip = layer;
        layerSource.loop = true;        // Music usually loops
        layerSource.volume = 0.0f;
        layerSource.priority = 1;     
        musicSourceLayers.Add(layerSource);
        layerSource.Play();
    }

    public void LayerScroll(float _hueValue) {
        // _hueValue is between 0,1
        // so we need 7 layers, with an additional 8th for violet->red

        const float numLayers = 7.0f;
        const float colorPoint = 1.0f/numLayers;

        float scaledHue = _hueValue * numLayers - 0.5f;
        print(scaledHue);
        int lowerIndex = (int)scaledHue;
        int upperIndex = (int) ((scaledHue + 1) % numLayers);



        print("Lower "+ lowerIndex + " and Upper " + upperIndex);

        musicSourceLayers[lowerIndex].volume = (upperIndex - scaledHue);
        print("Lower Volume: " + musicSourceLayers[lowerIndex].volume);
        print("Upper Volume: " + musicSourceLayers[upperIndex].volume);
        musicSourceLayers[upperIndex].volume = -(lowerIndex - scaledHue);
    }
}
