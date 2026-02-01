using UnityEngine;
using System.Collections;

public class TimedParticleController : MonoBehaviour
{
    [Header("Particle Settings")]
    public ParticleSystem particleSystemToControl;
    public float disableAfterSeconds = 2f;

    private Coroutine disableRoutine;

    void Awake()
    {
        if (particleSystemToControl == null)
            particleSystemToControl = GetComponent<ParticleSystem>();
    }

    // 🔥 Call from event or code
    public void PlayParticles()
    {
        if (particleSystemToControl == null) return;

        particleSystemToControl.Play();

        if (disableRoutine != null)
            StopCoroutine(disableRoutine);

        disableRoutine = StartCoroutine(DisableAfterTime());
    }

    // ❄️ Call from event or code
    public void StopParticles()
    {
        if (particleSystemToControl == null) return;

        if (disableRoutine != null)
        {
            StopCoroutine(disableRoutine);
            disableRoutine = null;
        }

        particleSystemToControl.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(disableAfterSeconds);

        particleSystemToControl.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        disableRoutine = null;
        gameObject.SetActive(false);
    }

}

