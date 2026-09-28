using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private Image cardBackgroundImage;

    public void InitializeCardUI(FighterActions action)
    {
        if (action == null)
        {
            Debug.LogWarning($"{name}: Received a null card action.");
            gameObject.SetActive(false);
            return;
        }

        if (cardNameText == null)
        {
            Debug.LogError(
                $"{name}: Card Name Text is not assigned on the CardUI prefab.");

            return;
        }

        cardNameText.text = action.name;
    }

    public void SetCardColor(Color newColor)
    {
        if (cardBackgroundImage == null)
        {
            Debug.LogWarning(
                $"{name}: Card Background Image is not assigned in the CardUI prefab.");

            return;
        }

        cardBackgroundImage.color = newColor;
    }
}
