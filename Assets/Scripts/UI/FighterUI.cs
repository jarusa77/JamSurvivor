using TMPro;
using UnityEngine;

public class FighterUI : MonoBehaviour
{

    private GameObject HealthContainer;
    private TextMeshProUGUI HealthText;

    private GameObject StaminaContainer;
    private TextMeshProUGUI StaminaText;
    private HealthBarUI _HealthBarUI;
    
    void Start()
    {
        HealthContainer = transform.GetChild(0).gameObject;
        HealthText = HealthContainer.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        StaminaContainer = transform.GetChild(1).gameObject;
        StaminaText = StaminaContainer.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _HealthBarUI = HealthContainer.transform.GetChild(2).GetComponent<HealthBarUI>();
    }

    public void InitializeValues(int maxHealth, int currentHealth, int maxStamina, int currentStamina)
    {
        _HealthBarUI.InitailizeValues(maxHealth, currentHealth);
        HealthText.text = currentHealth.ToString();
        
    }

    public void UpdateHealth(int health)
    {
        HealthText.text = health.ToString();
        _HealthBarUI.SetHealth(health);
    }

    public void UpdateStamina(int stamina)
    {
        StaminaText.text = stamina.ToString();
    }
}
