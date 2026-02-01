using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    
    private void Awake()
    {
        Timer.OnTimerUpdate += UpdateTimeUI;
    }

    private void UpdateTimeUI(float currenttime)
    {
        timerText.text = Mathf.FloorToInt(currenttime).ToString();
    }

    void Destroy()
    {
        Timer.OnTimerUpdate -= UpdateTimeUI;
    }
}
