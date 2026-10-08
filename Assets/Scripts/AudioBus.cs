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

        DefaultButtonHoverSFX = Resources.Load<AudioClip>("SFX/ButtonHover");
        DefaultButtonClickSFX = Resources.Load<AudioClip>("SFX/ButtonClick");
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
    private AudioSource _musicSource;
    private List<AudioSource> _musicSourceLayers = new List<AudioSource>();
    private AudioSource _sfxSource;

    void Start()
    {
        // Create and configure audio sources
        //SetupAudioSources();
    }

    float dialogueMult = 1f;

    void Update()
    {
        float dialogueMultTarg = DialogueSystem.Instance.IsPlaying ? 0.25f : 1f;
        float rate = DialogueSystem.Instance.IsPlaying ? 6f : 2f;
        dialogueMult = Mathf.Lerp(dialogueMult, dialogueMultTarg, rate * Time.deltaTime);

        _sfxSource.volume = GameSettings.SFXVolume;
        _musicSource.volume = GameSettings.MusicVolume * dialogueMult;

        LayerScroll(_lastHue);
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
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;        // Music usually loops
        _musicSource.volume = defaultMusicVolume;      // Lower volume for background music
        _musicSource.priority = 0;       // Highest priority

        // SFX audio source
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;         // Sound effects don't usually loop
        _sfxSource.volume = defaultSFXVolume;        // Full volume for sound effects
        _sfxSource.priority = 128;       // Normal priority
    }

    // Play a music track
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        // Stop any currently playing music
        _musicSource.Stop();

        // Set the new music clip
        _musicSource.clip = clip;
        _musicSource.loop = loop;

        // Start playing
        _musicSource.Play();
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

        _sfxSource.PlayOneShot(clip, volume);
    }

    // Control music volume
    public void SetMusicVolume(float volume = defaultMusicVolume)
    {
        _musicSource.volume = Mathf.Clamp01(volume);
    }

    // Control SFX volume
    public void SetSFXVolume(float volume)
    {
        _sfxSource.volume = Mathf.Clamp01(volume);
    }

    // Fade music in
    public void FadeMusicIn(float duration = 1.0f)
    {
        StartCoroutine(FadeMusicVolume(0, _musicSource.volume, duration));
    }

    // Fade music out
    public void FadeMusicOut(float duration = 1.0f)
    {
        StartCoroutine(FadeMusicVolume(_musicSource.volume, 0, duration));
    }

    // Coroutine to fade music volume
    private System.Collections.IEnumerator FadeMusicVolume(float startVolume, float targetVolume, float duration)
    {
        float startTime = Time.time;
        float elapsedTime = 0;

        _musicSource.volume = startVolume;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            float normalizedTime = elapsedTime / duration;
            _musicSource.volume = Mathf.Lerp(startVolume, targetVolume, normalizedTime);
            yield return null;
        }

        _musicSource.volume = targetVolume;
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
        _musicSourceLayers.Add(layerSource);
        layerSource.Play();
    }

    private float _lastHue;

    public void LayerScroll(float _hueValue)
    {
        _lastHue = _hueValue;
        int numLayers = _musicSourceLayers.Count;
        if (numLayers == 0) return;

        float scaledHue = _hueValue * numLayers - 0.5f;

        int lowerIndex = Mathf.FloorToInt(scaledHue);
        float t = scaledHue - lowerIndex;

        int lower = ((lowerIndex % numLayers) + numLayers) % numLayers;
        int upper = (lower + 1) % numLayers;

        for (int i = 0; i < numLayers; i++)
            _musicSourceLayers[i].volume = 0f;

        _musicSourceLayers[lower].volume = (1f - t) * GameSettings.MusicVolume * dialogueMult;
        _musicSourceLayers[upper].volume = t * GameSettings.MusicVolume * dialogueMult;
    }
}