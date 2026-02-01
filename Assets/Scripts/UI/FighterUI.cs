using TMPro;
using UnityEngine;

public class FighterUI : MonoBehaviour
{

    private GameObject HealthContainer;
    private TextMeshProUGUI HealthText;

    private GameObject StaminaContainer;
    private TextMeshProUGUI StaminaText;
    
    void Start()
    {
        HealthContainer = transform.GetChild(0).gameObject;
        HealthText = HealthContainer.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        StaminaContainer = transform.GetChild(1).gameObject;
        StaminaText = StaminaContainer.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void updateHealth(int health)
    {
        HealthText.text = health.ToString();
    }

    public void UpdateStamina(int stamina)
    {
        StaminaText.text = stamina.ToString();
    }
}
