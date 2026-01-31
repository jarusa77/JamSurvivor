using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


internal enum PlayerState{ Idle, TurnEnd, KO }
public class Fighter : MonoBehaviour
{
    //likely to have a player ID to know which card selection corresponds to whom(?)
    [SerializeField] int ID;
    [SerializeField] List<PlayerCardInHand> Hand;
    public int MaxHP = 100;
    private int CurrentHP;
    private static int PlayerMaxCards = 5;
    public static int MaxMana = 3;
    public int CurrentMana;

    internal List<Card> QueuedCards;

    public InputActionAsset inputActions;
    
    private InputAction option1;
    private InputAction option2;
    private InputAction option3;
    private InputAction option4;
    private InputAction option5;
    private InputAction turnEnd;
    
    
    

    internal List<Card> TurnCardsQueued = new List<Card>();
    internal PlayerState CurrentState = PlayerState.Idle;


    public delegate void PlayerTurnSet();
    public static event PlayerTurnSet OnPlayerTurnSet;

    public delegate void PlayerKO();
    public static event PlayerKO OnPlayerKO;

    private void Awake()
    {
        option1 = inputActions.FindAction("Option1");
        option2 = inputActions.FindAction("Option2");
        option3 = inputActions.FindAction("Option3");
        option4 = inputActions.FindAction("Option4");
        option5 = inputActions.FindAction("Option5");
        turnEnd = inputActions.FindAction("TurnEnd");
        Hand = new List<PlayerCardInHand>();
        QueuedCards = new List<Card>();
    }

    private void OnEnable()
    {
        inputActions.FindActionMap("MoveSelect").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("MoveSelect").Disable();
    }

    internal void DiscardHand()
    {
        foreach (PlayerCardInHand card in Hand)
        {
            DeckSystem.Instance.DiscardCard(card._card);
        }
        Hand.Clear();
    }

    void Start()
    {
        VariableInitialize();
        GameManager.Instance.AddFighter(this);
        
    }

    void VariableInitialize()
    {
        CurrentHP = MaxHP;
        CurrentMana = MaxMana;
    }

    void Update()
    {
        if(option1.WasPressedThisFrame())
            SelectCardForQueue(0);
        if(option2.WasPressedThisFrame())
            SelectCardForQueue(1);
        if(option3.WasPressedThisFrame())
            SelectCardForQueue(2);
        if(option4.WasPressedThisFrame())
            SelectCardForQueue(3);
        if (option5.WasPressedThisFrame())
            SelectCardForQueue(4);
        
        if(turnEnd.WasPressedThisFrame())
            EndTurn();
    }

    internal void DrawForTurn()
    {
        while (Hand.Count < PlayerMaxCards)
        {
            /*
            Card deepCopy = Instantiate((DeckSystem.Instance.Draw()));
            if(deepCopy != null)
                Hand.Add(deepCopy);
            */
            Hand.Add(new PlayerCardInHand(DeckSystem.Instance.Draw(), false));
        }

        CurrentState = PlayerState.Idle;
        CurrentMana = MaxMana;
        QueuedCards.Clear();
    }

    private void SelectCardForQueue(int index)
    {
        Debug.Log("Player "+ID+" picked "+index);
        Debug.Log(CurrentMana + " -> " + Hand[index]._card._ManaCost);
        if(!Hand[index]._isSelected)
        {
            if (Hand[index]._card._ManaCost > CurrentMana)
            {
                Debug.Log("Not enough Mana!");
                return;
            }
            else
            {
                //TODO: Might not need to save actions to a different queue, but can instead loop through the hand and pick the selected.
                Hand[index]._isSelected = true;
                CurrentMana -= Hand[index]._card._ManaCost;
                QueuedCards.Add(Hand[index]._card);
               //AddCardToQueue(Hand[index]);
            }
        }
        else
        {
            //player "de-selected" the card from the queue - hence returning the mana cost.
            Hand[index]._isSelected = false;
            CurrentMana += Hand[index]._card._ManaCost;
            //TODO : will need to validate deep copy and ID comparison so that if a player selects multiple of the same card, it will only remove that from the queue
            QueuedCards.Remove(Hand[index]._card);
        }
    }

    public void AddCardToQueue(Card pCard)
    {
        if(CurrentMana >= pCard._ManaCost)
        {
            TurnCardsQueued.Add(pCard);
        }
        else
        {
            Debug.Log("Not enough Mana to play card!");
            //Play sfx or something
        }
    }

    public void EndTurn()
    {
        CurrentState = PlayerState.TurnEnd;
        OnPlayerTurnSet?.Invoke();
    }

    public void ProcessBattleOutcome(AttackOutcome pOutcome)
    {
        //if fighter got hit by something
        if (pOutcome._SuccessfullAttack)
        {
            CurrentHP -= pOutcome._Damage;
        }
    }

    private void CheckForDeath()
    {
        if (CurrentHP <= 0)
        {
            CurrentState = PlayerState.KO;
            OnPlayerKO?.Invoke();
        }
    }


}

[Serializable] public class PlayerCardInHand
{
    [SerializeField] internal Card _card;
    [SerializeField] internal bool _isSelected;

    public PlayerCardInHand(Card card, bool isSelected)
    {
        _card = card;
        _isSelected = isSelected;
    }
}
