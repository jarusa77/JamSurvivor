using UnityEngine;

public class CharSFX : MonoBehaviour
{
    //Sounds
    public AudioClip kickSFX;
    public AudioClip punchSFX;
    public AudioClip BlockSFX;
    public AudioClip hitSFX;
    public AudioClip DodgeSFX;

    public AudioClip DashInSFX;

    public AudioClip winSFX;

    public AudioClip startSFX;

    SoundManager soundManager;

    private void OnEnable()
    {
        soundManager = SoundManager.Instance;
    }

    // -------------------------
    // Animation Event SFX
    // -------------------------

    public void PlayKickSFX()
    {
        PlaySFXSafe(kickSFX);
    }

    public void PlayPunchSFX()
    {
        PlaySFXSafe(punchSFX);
    }

    public void PlayBlockSFX()
    {
        PlaySFXSafe(BlockSFX);
    }

    public void PlayDodgeSFX()
    {
        PlaySFXSafe(DodgeSFX);
    }

    public void PlayWinSFX()
    {
        PlaySFXSafe(winSFX);
    }

    public void PlayHitSFX()
    {
        PlaySFXSafe(hitSFX);
    }

    public void PlayDashSFX()
    {
        PlaySFXSafe(DashInSFX);
    }

    public void PlayStartSFX()
    {
        PlaySFXSafe(startSFX);
    }

    private void PlaySFXSafe(AudioClip clip)
    {
        if (clip == null) return;
        if (soundManager == null) return;

        soundManager.PlaySound(clip);
    }
}
