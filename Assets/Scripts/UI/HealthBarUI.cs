using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fillImage;

    [Header("Animation")]
    [SerializeField] private float healthBarSpeed = 2.5f;

    [Header("Health Colors")]
    [SerializeField] private Color highHealthColor = Color.green;
    [SerializeField] private Color warningHealthColor = Color.yellow;
    [SerializeField] private Color criticalHealthColor = Color.red;

    private float maxHealth = 100f;
    private float targetHealth;
    private float displayedHealth;

    private float highHealthThreshold;
    private float warningHealthThreshold;

    private void Awake()
    {
        // Best option: assign Fill Image manually in the Inspector.
        // Backup option: automatically use the first child Image.
        if (fillImage == null && transform.childCount > 0)
        {
            fillImage = transform.GetChild(0).GetComponent<Image>();
        }

        if (fillImage == null)
        {
            Debug.LogError(
                $"{name}: HealthBarUI has no Fill Image assigned. " +
                "Assign an Image component in the Inspector.");

            return;
        }

        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = 0;
    }

    private void Update()
    {
        if (fillImage == null)
        {
            return;
        }

        // Smoothly moves displayed health toward the real target health.
        displayedHealth = Mathf.MoveTowards(
            displayedHealth,
            targetHealth,
            healthBarSpeed * maxHealth * Time.deltaTime);

        UpdateBarVisual();
    }

    public void InitializeValues(int newMaxHealth, int newCurrentHealth)
    {
        if (fillImage == null)
        {
            Debug.LogError($"{name}: Cannot initialize because Fill Image is missing.");
            return;
        }

        maxHealth = Mathf.Max(1f, newMaxHealth);

        targetHealth = Mathf.Clamp(newCurrentHealth, 0f, maxHealth);
        displayedHealth = targetHealth;

        highHealthThreshold = maxHealth * 0.67f;
        warningHealthThreshold = maxHealth * 0.33f;

        UpdateBarVisual();
    }

    public void SetHealth(float newHealth)
    {
        targetHealth = Mathf.Clamp(newHealth, 0f, maxHealth);

        UpdateHealthColor();
    }

    private void UpdateBarVisual()
    {
        fillImage.fillAmount = displayedHealth / maxHealth;

        UpdateHealthColor();
    }

    private void UpdateHealthColor()
    {
        if (targetHealth >= highHealthThreshold)
        {
            fillImage.color = highHealthColor;
        }
        else if (targetHealth >= warningHealthThreshold)
        {
            fillImage.color = warningHealthColor;
        }
        else
        {
            fillImage.color = criticalHealthColor;
        }
    }
}
