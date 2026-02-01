using UnityEngine;

public class EventFXController : MonoBehaviour
{
    public enum FXMode
    {
        ParticleSystem,
        Prefab
    }

    [Header("FX Type")]
    public FXMode fxMode = FXMode.ParticleSystem;

    [Header("Particle System")]
    public ParticleSystem particleSystemToControl;

    [Header("Prefab FX")]
    public GameObject fxPrefab;
    public Transform spawnPoint;
    public bool destroyOnDisable = true;

    private GameObject spawnedFX;

    void Awake()
    {
        if (spawnPoint == null)
            spawnPoint = transform;
    }

    // ?? CALL FROM EVENT
    public void TurnOnFX()
    {
        if (fxMode == FXMode.ParticleSystem)
        {
            if (particleSystemToControl == null) return;

            if (!particleSystemToControl.isPlaying)
                particleSystemToControl.Play();
        }
        else
        {
            if (fxPrefab == null) return;

            if (spawnedFX == null)
            {
                spawnedFX = Instantiate(fxPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            }
            else
            {
                spawnedFX.SetActive(true);
            }
        }
    }

    // ?? CALL FROM EVENT
    public void TurnOffFX()
    {
        if (fxMode == FXMode.ParticleSystem)
        {
            if (particleSystemToControl == null) return;

            particleSystemToControl.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else
        {
            if (spawnedFX == null) return;

            if (destroyOnDisable)
            {
                Destroy(spawnedFX);
                spawnedFX = null;
            }
            else
            {
                spawnedFX.SetActive(false);
            }
        }
    }

    // ?? OPTIONAL
    public void ToggleFX()
    {
        if (fxMode == FXMode.ParticleSystem)
        {
            if (particleSystemToControl == null) return;

            if (particleSystemToControl.isPlaying)
                TurnOffFX();
            else
                TurnOnFX();
        }
        else
        {
            if (spawnedFX == null || !spawnedFX.activeSelf)
                TurnOnFX();
            else
                TurnOffFX();
        }
    }
}
