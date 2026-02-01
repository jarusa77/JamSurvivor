using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance;
    
    [SerializeField] private float roundTime = 60f;
    private bool autoStart = false;

    public delegate void TimerUpdate(float currentTime);
    public static event TimerUpdate OnTimerUpdate;
    
    public delegate void TimerEnd();
    public static event TimerEnd OnTimerEnd;
    
    private float currentTime;
    private bool isRunning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentTime = roundTime;

        if (autoStart)
            StartTimer();

        NotifyTimeChanged();
    }

    public void ResetTimer()
    {
        currentTime = roundTime;
        NotifyTimeChanged();
        StartTimer();
    }

    public void FightBegin()
    {
        autoStart = true;
        ResetTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        float previousTime = currentTime;

        currentTime -= Time.deltaTime;
        currentTime = Mathf.Max(0, currentTime);

        if (Mathf.FloorToInt(previousTime) != Mathf.FloorToInt(currentTime))
            NotifyTimeChanged();

        if (currentTime <= 0 && isRunning)
        {
            isRunning = false;
            OnTimerEnd?.Invoke();
        }
    }

    void NotifyTimeChanged()
    {
        OnTimerUpdate?.Invoke(currentTime);
    }

    void StartTimer()
    {
        isRunning = true;
    }
}
