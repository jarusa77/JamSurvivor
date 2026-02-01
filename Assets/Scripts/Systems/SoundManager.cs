using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource _musicSource, _sound, _tempSound, _ambientSound;

    [SerializeField] private AudioMixer Mixer;
    [SerializeField] private AudioMixerGroup MusicGroup, SFXGroup;

    private bool MusicMuted = false;
    [SerializeField] private float MusicVolume = 0.5f;

    private bool SFXMuted = false;
    private float SFXVolume = 0.5f;

    [SerializeField] private float fadeSpeed = 0.5f;
    private float volume = 0.5f;

    private bool isFadeIn, isFadeOut;

    private void Awake()
    {
        // Route outputs
        if (MusicGroup != null) _musicSource.outputAudioMixerGroup = MusicGroup;
        if (SFXGroup != null)
        {
            _sound.outputAudioMixerGroup = SFXGroup;
            _tempSound.outputAudioMixerGroup = SFXGroup;
            _ambientSound.outputAudioMixerGroup = SFXGroup;
        }

        // Keep sources at unity gain — all loudness via mixer
        if (_musicSource != null) _musicSource.volume = 1f;
        if (_sound != null) _sound.volume = 1f;
        if (_tempSound != null) _tempSound.volume = 1f;
        if (_ambientSound != null) _ambientSound.volume = 1f;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Music Volume"))
            setMusicVolume(PlayerPrefs.GetFloat("Music Volume"));

        if (PlayerPrefs.HasKey("SFX Volume"))
            setSoundVolume(PlayerPrefs.GetFloat("SFX Volume"));
    }

    // --- Helper to prep music source and ensure unity gain on the AudioSource
    private void PrepMusicSource(AudioClip clip)
    {
        _musicSource.outputAudioMixerGroup = MusicGroup;
        _musicSource.volume = 1f; // critical: never leave this at a reduced value
        _musicSource.clip = clip;
    }

    public void PlayMusic(AudioClip clip, bool ForceRestart = false)
    {
        if (_musicSource.clip == clip && _musicSource.isPlaying && ForceRestart)
            return;

        Debug.Log("Playing Clip " + clip.name);

        StartCoroutine(FadeMusicTransition(clip, fadeSpeed));
    }

    // One-shot music (non-loop) without touching AudioSource.volume
    public void PlayMusicOnce(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;

        _musicSource.loop = false;
        PrepMusicSource(clip);

        // Drive loudness via mixer only (linear -> dB)
        float linear = Mathf.Clamp01(MusicVolume * volumeScale);
        Mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(linear, 0.001f)) * 20);

        _musicSource.Play();
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (_ambientSound.clip == clip && _ambientSound.isPlaying)
            return;

        StartCoroutine(FadeAmbientTransition(clip, fadeSpeed));
    }

    public void PlaySound(AudioClip clip)
    {
        _sound.PlayOneShot(clip);
    }

    public void PlaySound(AudioClip clip, float scale)
    {
        _sound.PlayOneShot(clip, scale);
    }

    public void setMusicVolume(float input)
    {
        MusicVolume = input;
        PlayerPrefs.SetFloat("Music Volume", input);
        MusicMuted = false;
        SetMixerVolume("MusicVolume", MusicVolume);
    }

    public float getMusicVolume() => MusicVolume;

    public void muteMusic()
    {
        MusicMuted = !MusicMuted;
        if (MusicMuted)
            Mixer.SetFloat("MusicVolume", -100f);
        else
            SetMixerVolume("MusicVolume", MusicVolume);
    }

    public bool isMusicMuted() => MusicMuted;

    public void setSoundVolume(float input)
    {
        SFXVolume = input;
        PlayerPrefs.SetFloat("SFX Volume", input);
        SFXMuted = false;
        SetMixerVolume("SFXVolume", SFXVolume);
    }

    public float getSoundVolume() => SFXVolume;

    public void muteSound()
    {
        SFXMuted = !SFXMuted;
        Mixer.SetFloat("SFXVolume", SFXMuted ? -100f : Mathf.Log10(Mathf.Max(SFXVolume, 0.001f)) * 20);
    }

    public bool isSFXMuted() => SFXMuted;

    private void SetMixerVolume(string parameter, float value)
    {
        float safeVolume = Mathf.Max(0.001f, value);
        Mixer.SetFloat(parameter, Mathf.Log10(safeVolume) * 20);
    }

    IEnumerator FadeMusicTransition(AudioClip newSong, float speed)
    {
        if (MusicMuted)
        {
            // Stay muted on the mixer; just swap the clip and play
            PrepMusicSource(newSong);
            _musicSource.Play();
            yield break;
        }

        volume = MusicVolume;

        if (_musicSource.clip != null)
            yield return StartCoroutine(FadeOutMusic(speed));

        yield return StartCoroutine(FadeInMusic(newSong, speed));
    }

    IEnumerator FadeInMusic(AudioClip newSong, float speed)
    {
        isFadeIn = true;

        PrepMusicSource(newSong);

        // Start fully muted at the mixer and fade up
        Mixer.SetFloat("MusicVolume", -100f);
        _musicSource.Play();

        float v = 0f;
        while (v < MusicVolume)
        {
            v += speed * Time.unscaledDeltaTime;
            SetMixerVolume("MusicVolume", v);
            yield return null;
        }

        isFadeIn = false;
    }

    IEnumerator FadeOutMusic(float speed)
    {
        isFadeOut = true;

        float v = MusicVolume;
        while (v > 0f)
        {
            v -= speed * Time.unscaledDeltaTime;
            SetMixerVolume("MusicVolume", v);
            yield return null;
        }

        Mixer.SetFloat("MusicVolume", -100f);
        isFadeOut = false;
    }

    IEnumerator FadeAmbientTransition(AudioClip newClip, float speed)
    {
        // NOTE: ambient still fades via source volume to avoid affecting other SFX
        float targetVolume = Mathf.Clamp01(SFXVolume);

        if (_ambientSound.isPlaying)
        {
            while (_ambientSound.volume > 0f)
            {
                _ambientSound.volume -= speed * Time.unscaledDeltaTime;
                yield return null;
            }
        }

        _ambientSound.clip = newClip;
        _ambientSound.Play();

        while (_ambientSound.volume < targetVolume)
        {
            _ambientSound.volume += speed * Time.unscaledDeltaTime;
            yield return null;
        }

        _ambientSound.volume = targetVolume;
    }

    public void StopMusic(bool immediate = false)
    {
        if (immediate)
        {
            Debug.Log("Immediate Music Stop");
            _musicSource.Stop();
            Mixer.SetFloat("MusicVolume", -100f);
        }
        else
        {
            Debug.Log("Fade Music Stop");
            StartCoroutine(FadeOutAndStopMusic(fadeSpeed));
        }
    }

    IEnumerator FadeOutAndStopMusic(float speed)
    {
        isFadeOut = true;
        float v = MusicVolume;

        while (v > 0f)
        {
            v -= speed * Time.unscaledDeltaTime;
            SetMixerVolume("MusicVolume", v);
            yield return null;
        }

        _musicSource.Stop();
        Mixer.SetFloat("MusicVolume", -100f);
        isFadeOut = false;
    }

    public void StopAmbient(bool immediate = false)
    {
        if (immediate)
        {
            _ambientSound.Stop();
            _ambientSound.volume = 0f;
        }
        else
        {
            StartCoroutine(FadeOutAndStopAmbient(fadeSpeed));
        }
    }

    IEnumerator FadeOutAndStopAmbient(float speed)
    {
        while (_ambientSound.volume > 0f)
        {
            _ambientSound.volume -= speed * Time.unscaledDeltaTime;
            yield return null;
        }

        _ambientSound.Stop();
        _ambientSound.volume = 0f;
    }
}
