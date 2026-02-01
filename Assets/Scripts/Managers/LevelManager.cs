using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public AudioClip Music;
    SoundManager soundManager;

    void Start()
    {
        soundManager = SoundManager.Instance;
        if (Music != null)
        {
            soundManager.PlayMusic(Music);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
