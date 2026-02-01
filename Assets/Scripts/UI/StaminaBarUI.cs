using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Image fillImage;

    private float MaxStamina = 100f;
    private float CurrentStamina;

    public void InitailizeValues(int maxStamina, int currentStamina)
    {
        MaxStamina = maxStamina;
        CurrentStamina = currentStamina;
        UpdateBar();
    }

    void Start()
    {
        fillImage = transform.GetChild(1).GetComponent<Image>();
    }

    public void SetStamina(float stamina)
    {
        CurrentStamina = Mathf.Clamp(stamina, 0, MaxStamina);
        UpdateBar();
    }

    void UpdateBar()
    {
        fillImage.fillAmount = CurrentStamina / MaxStamina;
    }
}
