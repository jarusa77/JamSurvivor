using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardUI : MonoBehaviour
{

    public GameObject AttackImageContainer;
    public GameObject StaminaContainer;

    private int CardIndex;
    private int PlayerID;

    public void InitializeCardUI(FighterActions action, int playerID, int cardIndex)
    {
        PlayerID = playerID;
        CardIndex = cardIndex;
        AttackImageContainer = this.transform.GetChild(0).gameObject;
        StaminaContainer = this.transform.GetChild(1).gameObject;
        
        if(AttackImageContainer)
            PopulateAttackContainer(action.TypeSprite, action._ActionType);
        if(StaminaContainer)
            PopulateStaminaContainer(action._ManaCost);
            
    }

    private void PopulateStaminaContainer(int staminaCost)
    {
        StaminaContainer.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = staminaCost.ToString();
    }

    private void PopulateAttackContainer(Sprite spriteImage, ActionType actionType)
    {
        UnityEngine.UI.Image actionImage = AttackImageContainer.GetComponent<UnityEngine.UI.Image>();
        actionImage.sprite = spriteImage;
        AttackImageContainer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = actionType.ToString().ToUpper();
    }

    public void CardClicked()
    {
        GameManager.Instance.AddCardToFighterQueue(PlayerID, CardIndex);
    }
}
