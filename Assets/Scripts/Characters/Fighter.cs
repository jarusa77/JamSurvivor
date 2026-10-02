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
    [SerializeField] private  int PlayerMaxCards = 5;
    [SerializeField] private int MaxMana = 3;
    public int CurrentMana;

    

    public List<FighterActions> QueuedCards;

    public InputActionAsset inputActions;
    
    private InputAction option1;
    private InputAction option2;
    private InputAction option3;
    private InputAction option4;
    private InputAction option5;
    private InputAction turnEnd;

    [SerializeField] HandUI HandContainerUI;
    [SerializeField] private BattleCardUI BattleContainerUI;
    [SerializeField] private FighterUI _FighterUI;
    [SerializeField] private bool isAIControlled = false;
    [SerializeField] private string inputActionMapName = "Player1Select";

    public bool IsAIControlled => isAIControlled;
    public int AvailableMana => CurrentMana;
    public List<PlayerCardInHand> CurrentHand => Hand;


    internal List<FighterActions> TurnCardsQueued = new List<FighterActions>();
    internal PlayerState CurrentState = PlayerState.Idle;


    public delegate void PlayerTurnSet();
    public static event PlayerTurnSet OnPlayerTurnSet;

    public delegate void PlayerKO();
    public static event PlayerKO OnPlayerKO;

    internal int GetHP()
    {
        return CurrentHP;
    }

    internal int GetID()
    {
        return ID;
    }

    private void Awake()
    {
        InputActionMap actionMap = inputActions.FindActionMap(inputActionMapName);

        if (actionMap == null)
        {
            Debug.LogError($"Fighter {ID} cannot find input map: {inputActionMapName}");
            return;
        }

        option1 = actionMap.FindAction("Option1");
        option2 = actionMap.FindAction("Option2");
        option3 = actionMap.FindAction("Option3");
        option4 = actionMap.FindAction("Option4");
        option5 = actionMap.FindAction("Option5");
        turnEnd = actionMap.FindAction("TurnEnd");

        Hand = new List<PlayerCardInHand>();
        QueuedCards = new List<FighterActions>();

        Timer.OnTimerEnd += AutoSetQueue;
        GameManager.OnToggleFighterInput += ToggleFighterInput;
    }
    void ToggleFighterInput(bool isActive)
    {
        if (isAIControlled)
        {
            inputActions.FindActionMap(inputActionMapName)?.Disable();
            return;
        }

        InputActionMap actionMap = inputActions.FindActionMap(inputActionMapName);

        if (actionMap == null)
        {
            Debug.LogError($"Input map '{inputActionMapName}' was not found for Fighter {ID}.");
            return;
        }

        if (isActive)
            actionMap.Enable();
        else
            actionMap.Disable();
    }
    public void SetAIControlled(bool value)
    {
        isAIControlled = value;

        InputActionMap actionMap = inputActions.FindActionMap(inputActionMapName);

        if (isAIControlled)
        {
            actionMap?.Disable();
        }
    }
    public bool TryQueueCard(int index)
    {
        if (CurrentState != PlayerState.Idle)
            return false;

        if (index < 0 || index >= Hand.Count)
            return false;

        PlayerCardInHand selectedCard = Hand[index];

        if (selectedCard._isSelected)
            return false;

        if (selectedCard._card == null)
            return false;

        if (selectedCard._card._ManaCost > CurrentMana)
            return false;

        selectedCard._isSelected = true;
        CurrentMana -= selectedCard._card._ManaCost;
        QueuedCards.Add(selectedCard._card);

        _FighterUI.UpdateStamina(CurrentMana);

        return true;
    }
    private void OnDestroy()
    {
        Timer.OnTimerEnd -= AutoSetQueue;
    }

    private void AutoSetQueue()
    {
        EndTurn();
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
        HandContainerUI.CreatePlaceholders(PlayerMaxCards);
        GameManager.Instance.AddFighter(this);
    }

    void VariableInitialize()
    {
        CurrentHP = MaxHP;
        CurrentMana = MaxMana;
        _FighterUI.InitializeValues(MaxHP, CurrentHP, MaxMana, CurrentMana);
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
        if (isAIControlled)
            return;

        if (option1.WasPressedThisFrame())
            SelectCardForQueue(0);

        if (option2.WasPressedThisFrame())
            SelectCardForQueue(1);

        if (option3.WasPressedThisFrame())
            SelectCardForQueue(2);

        if (option4.WasPressedThisFrame())
            SelectCardForQueue(3);

        if (option5.WasPressedThisFrame())
            SelectCardForQueue(4);

        if (turnEnd.WasPressedThisFrame())
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
        _FighterUI.UpdateStamina(CurrentMana);
        QueuedCards.Clear();
        
        HandContainerUI.PopulateHandUI(Hand);
    }

   

    public void AddCardToQueue(FighterActions pCard)
    {
        if (CurrentMana >= pCard._ManaCost)
        {
            CurrentMana -= pCard._ManaCost;
            QueuedCards.Add(pCard);
            _FighterUI.UpdateStamina(CurrentMana);
        }
        else
        {
            Debug.Log("Not enough Mana to play card!");
        }
    }
    private void SelectCardForQueue(int index)
    {
        if (!TryQueueCard(index))
        {
            Debug.Log("Card could not be queued.");
        }
    }
    public void EndTurn()
    {
        if (CurrentState != PlayerState.Idle)
            return;

        CurrentState = PlayerState.TurnEnd;

        BattleContainerUI.ClearQueue();
        BattleContainerUI.AddQueuedCardsToUI(QueuedCards);

        OnPlayerTurnSet?.Invoke();
    }

    public void ProcessBattleOutcome(ActionData pOutcome)
    {
        //Debug.Log("Player: "+ID+" will take "+pOutcome.Damage+" damage");
        CurrentHP -= pOutcome.Damage;
        _FighterUI.UpdateHealth(CurrentHP);
        CheckForDeath();
        
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
    [SerializeField] internal FighterActions _card;
    [SerializeField] internal bool _isSelected;

    public PlayerCardInHand(FighterActions card, bool isSelected)
    {
        _card = card;
        _isSelected = isSelected;
    }
}
