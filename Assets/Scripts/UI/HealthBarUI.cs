using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Image fillImage;

    private float MaxHealth = 100f;
    private float CurrentHealth;
    
    [SerializeField] private Color HighHealthColor;
    [SerializeField] private Color WarningHealthColor;
    [SerializeField] private Color CriticalHealthColor;

    private float highHealthThreshold;
    private float warningHealthThreshold;
    private float lowHealthThreshold;
    

    public void InitailizeValues(int maxHealth, int currentHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        
        highHealthThreshold = maxHealth * 0.67f;
        warningHealthThreshold = maxHealth * 0.66f;
        lowHealthThreshold = maxHealth * 0.33f;
        
        
        
        UpdateBar();
    }

    void Start()
    {
        fillImage = transform.GetChild(0).GetComponent<Image>();
        fillImage.color = HighHealthColor;
    }

    public void SetHealth(float health)
    {
        CurrentHealth = Mathf.Clamp(health, 0, MaxHealth);
        /*
        if (CurrentHealth >= highHealthThreshold)
        {
            fillImage.color = HighHealthColor;
        }
        else if (CurrentHealth < 66 && CurrentHealth >= warningHealthThreshold)
        {
            fillImage.color = WarningHealthColor;
        }
        else
        {
            fillImage.color = CriticalHealthColor;
        }
        */

        UpdateBar();
    }

    void UpdateBar()
    {
        fillImage.fillAmount = CurrentHealth / MaxHealth;
    }
}
