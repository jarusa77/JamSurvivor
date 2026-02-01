using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 0.5f;
    public float magnitude = 0.3f;

    private Vector3 originalPosition;
    private Coroutine shakeRoutine;

    void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void TriggerShake()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * magnitude;
            transform.localPosition = originalPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeRoutine = null;
    }
    private void Update()
    {
       
   
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TriggerShake();
        }

    
    }
}

