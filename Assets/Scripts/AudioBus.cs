using UnityEngine;
using System.Collections.Generic;

public class AudioBus : MonoBehaviour
{
    public static AudioBus Instance { get; private set; }
    const float defaultMusicVolume = 0.5f;
    const float defaultSFXVolume = 1.0f;
    public float pitchRandom = 0.15f;

    // UI Button SFX
    public static AudioClip DefaultButtonHoverSFX { get; private set; }
    public static AudioClip DefaultButtonClickSFX { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DefaultButtonHoverSFX = Resources.Load<AudioClip>("ButtonHover");
        DefaultButtonClickSFX = Resources.Load<AudioClip>("ButtonClick");
        SetupAudioSources();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            SpectrumManager.OnColorUpdate -= LayerScroll;
        }
    }

    // Audio source references
    private AudioSource musicSource;
    private List<AudioSource> musicSourceLayers = new List<AudioSource>();
    private AudioSource sfxSource;

    void Start()
    {
        // Create and configure audio sources
        //SetupAudioSources();
    }

    public void SubscribeToLayerScroll()
    {
        SpectrumManager.OnColorUpdate -= LayerScroll; // avoid double-subscribing
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

    public AudioSource PlaySFX(AudioClip clip, float volume = 1.0f, bool pitchRandomization = true, bool persistAcrossScenes = true)
    {
        if (clip == null) return null;

        GameObject soundSource = new GameObject($"SFX_{clip.name}");
        soundSource.transform.position = transform.position;

        if (persistAcrossScenes)
            soundSource.transform.SetParent(transform);

        AudioSource source = soundSource.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.loop = false;
        source.pitch = pitchRandomization
            ? Random.Range(1.0f - pitchRandom, 1.0f + pitchRandom)
            : 1.0f;
        source.Play();

        // Higher pitch makes the clip finish sooner.
        Destroy(soundSource, clip.length / source.pitch);
        return source;
    }

    public AudioSource PlayRandomSFX(AudioClip[] clips, bool persistAcrossScenes = true)
    {
        if (clips == null || clips.Length == 0) return null;
        return PlaySFX(clips[Random.Range(0, clips.Length)], 1.0f, true, persistAcrossScenes);
    }

    // Plays through the bus's own sfxSource, so it always persists.
    public void PlayOneshotSFX(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip, volume);
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

    public void InitMusicLayer(AudioClip layer)
    {
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

    public void LayerScroll(float _hueValue)
    {
        int numLayers = musicSourceLayers.Count;
        if (numLayers == 0) return;

        float scaledHue = _hueValue * numLayers - 0.5f;

        int lowerIndex = Mathf.FloorToInt(scaledHue);
        float t = scaledHue - lowerIndex;

        int lower = ((lowerIndex % numLayers) + numLayers) % numLayers;
        int upper = (lower + 1) % numLayers;

        for (int i = 0; i < numLayers; i++)
            musicSourceLayers[i].volume = 0f;

        musicSourceLayers[lower].volume = 1f - t;
        musicSourceLayers[upper].volume = t;
    }
}