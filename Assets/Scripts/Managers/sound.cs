using UnityEngine;

public class GameEventTrigger : MonoBehaviour
{
    public AudioEventPlayer audioPlayer;

    public void OnBossSpawn()
    {
        audioPlayer.PlayEvent("BossMusic");
    }

    public void OnExplosion()
    {
        audioPlayer.PlayOneShotEvent("Explosion");
    }

    public void OnPlayerDeath()
    {
        audioPlayer.PlayEvent("DeathTheme");
    }
}
