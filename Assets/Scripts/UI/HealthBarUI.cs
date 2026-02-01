using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Image fillImage;

    private float MaxHealth = 100f;
    private float CurrentHealth;

    public void InitailizeValues(int maxHealth, int currentHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        UpdateBar();
    }

    void Start()
    {
        fillImage = transform.GetChild(0).GetComponent<Image>();
    }

    public void SetHealth(float health)
    {
        CurrentHealth = Mathf.Clamp(health, 0, MaxHealth);
        UpdateBar();
    }

    void UpdateBar()
    {
        fillImage.fillAmount = CurrentHealth / MaxHealth;
    }
}
