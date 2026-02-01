using UnityEngine;
using System.Collections.Generic;

public class AnimationAudioHandler : MonoBehaviour
{
    [System.Serializable]
    public class AnimationSound
    {
        public string animationEventName;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    public List<AnimationSound> animationSounds = new List<AnimationSound>();

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Called from Animation Events
    public void PlayAnimationSound(string FightIdle)
    {
        AnimationSound sound = animationSounds.Find(s => s.animationEventName == FightIdle);

        if (sound == null || sound.clip == null)
        {
            Debug.LogWarning("Female  pain scream  sound  Effects 8: " + FightIdle);
            return;
        }

        audioSource.PlayOneShot(sound.clip, sound.volume);
    }
}
