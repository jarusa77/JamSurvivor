using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardUI : MonoBehaviour
{

    private GameObject AttackImageContainer;
    private GameObject StaminaContainer;
    private GameObject KeyStrokeContainer;

    private static readonly string[] p1 = { "1", "2", "3", "4", "5" };
    private static readonly string[] p2 = { "6", "7", "8", "9", "0" };
    

    public void InitializeCardUI(FighterActions action, int player, int handIndex)
    {
        AttackImageContainer = this.transform.GetChild(0).gameObject;
        StaminaContainer = this.transform.GetChild(1).gameObject;
        KeyStrokeContainer = this.transform.GetChild(2).gameObject;
        
        if(AttackImageContainer)
            PopulateAttackContainer(action.TypeSprite, action._ActionType);
        if(StaminaContainer)
            PopulateStaminaContainer(action._ManaCost);
        //hard check if it's comming from battlcard UI - unsure if said UI will be implemented; we do not need the keystrokes on BattleUI
        if(player == -1)
            return;
        if (KeyStrokeContainer)
            PopulateKeyStroke(player, handIndex);

    }

    private void PopulateKeyStroke(int playerID, int handIndex)
    {
        string[] selectedArray = playerID <= 1 ? p1 : p2;
        KeyStrokeContainer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = selectedArray[handIndex];
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
}
