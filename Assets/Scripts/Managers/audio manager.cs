using UnityEngine;
using System.Collections.Generic;

public class AudioEventPlayer : MonoBehaviour
{
    [System.Serializable]
    public class AudioEvent
    {
        public string eventName;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public bool loop;
    }

    public List<AudioEvent> audioEvents = new List<AudioEvent>();

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayEvent(string eventName)
    {
        AudioEvent audioEvent = audioEvents.Find(e => e.eventName == eventName);

        if (audioEvent == null)
        {
            Debug.LogWarning("Audio Event not found: " + eventName);
            return;
        }

        audioSource.clip = audioEvent.clip;
        audioSource.volume = audioEvent.volume;
        audioSource.loop = audioEvent.loop;
        audioSource.Play();
    }

    public void PlayOneShotEvent(string eventName)
    {
        AudioEvent audioEvent = audioEvents.Find(e => e.eventName == eventName);

        if (audioEvent == null)
        {
            Debug.LogWarning("Audio Event not found: " + eventName);
            return;
        }

        audioSource.PlayOneShot(audioEvent.clip, audioEvent.volume);
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }
}

