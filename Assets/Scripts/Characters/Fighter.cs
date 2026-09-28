using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

internal enum PlayerState
{
    Idle,
    TurnEnd,
    KO
}

public class Fighter : MonoBehaviour
{
    [Header("Fighter Settings")]
    [SerializeField] private int id;
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int playerMaxCards = 5;
    [SerializeField] private int maxMana = 3;

    [Header("Control Type")]
    [SerializeField] private bool isAIControlled = false;

    [Header("AI Settings")]
    [SerializeField] private float aiThinkDelay = 0.75f;
    [SerializeField] private int aiMinimumCardsToPlay = 1;
    [SerializeField] private int aiMaximumCardsToPlay = 3;

    [Header("Input - Human Only")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string inputActionMapName = "MoveSelect";

    [Header("UI")]
    [SerializeField] private HandUI handContainerUI;
    [SerializeField] private BattleCardUI battleContainerUI;
    [SerializeField] private FighterUI fighterUI;
    [SerializeField] List<PlayerCardInHand> Hand;
    [SerializeField]
    private List<PlayerCardInHand> hand =
        new List<PlayerCardInHand>();

    public List<FighterActions> QueuedCards { get; private set; } =
        new List<FighterActions>();

    private InputActionMap moveSelectMap;
    private InputAction option1;
    private InputAction option2;
    private InputAction option3;
    private InputAction option4;
    private InputAction option5;
    private InputAction turnEnd;

    private Coroutine aiTurnRoutine;

    private int currentHP;
    private int currentMana;

    internal PlayerState CurrentState { get; private set; } = PlayerState.Idle;

    public delegate void PlayerTurnSet();
    public static event PlayerTurnSet OnPlayerTurnSet;

    public delegate void PlayerKO();
    public static event PlayerKO OnPlayerKO;


    internal int GetHP()
    {
        return currentHP;
    }

    internal int GetID()
    {
        return id;
    }

    private void Awake()
    {
        hand ??= new List<PlayerCardInHand>();
        QueuedCards ??= new List<FighterActions>();

        if (!isAIControlled)
        {
            SetupHumanInput();
        }

        Timer.OnTimerEnd += AutoSetQueue;
        GameManager.OnToggleFighterInput += ToggleFighterInput;
    }

    private void Start()
    {
        InitializeFighterValues();

        if (handContainerUI != null)
        {
            handContainerUI.CreatePlaceholders(playerMaxCards);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddFighter(this);
        }
        else
        {
            Debug.LogError($"{name}: No GameManager was found in the scene.");
        }
    }

    private void OnDestroy()
    {
        StopAITurn();

        Timer.OnTimerEnd -= AutoSetQueue;
        GameManager.OnToggleFighterInput -= ToggleFighterInput;
    }

    private void SetupHumanInput()
    {
        if (inputActions == null)
        {
            Debug.LogError($"{name}: Assign an Input Action Asset for the human fighter.");
            return;
        }

        moveSelectMap = inputActions.FindActionMap(inputActionMapName);

        if (moveSelectMap == null)
        {
            Debug.LogError(
                $"{name}: Could not find input action map '{inputActionMapName}'.");

            return;
        }

        option1 = moveSelectMap.FindAction("Option1");
        option2 = moveSelectMap.FindAction("Option2");
        option3 = moveSelectMap.FindAction("Option3");
        option4 = moveSelectMap.FindAction("Option4");
        option5 = moveSelectMap.FindAction("Option5");
        turnEnd = moveSelectMap.FindAction("TurnEnd");

        if (option1 == null || option2 == null || option3 == null ||
            option4 == null || option5 == null || turnEnd == null)
        {
            Debug.LogError(
                $"{name}: One or more actions are missing from '{inputActionMapName}'.");
        }
    }

    private void InitializeFighterValues()
    {
        currentHP = maxHP;
        currentMana = maxMana;

        if (fighterUI != null)
        {
            fighterUI.InitializeValues(
                maxHP,
                currentHP,
                maxMana,
                currentMana);
        }
    }

    private void Update()
    {
        // AI does not respond to human keyboard/controller controls.
        if (isAIControlled)
        {
            return;
        }

        // Human can only select cards while their turn is active.
        if (CurrentState != PlayerState.Idle || moveSelectMap == null ||
            !moveSelectMap.enabled)
        {
            return;
        }

        if (option1 != null && option1.WasPressedThisFrame())
        {
            SelectCardForQueue(0);
        }

        if (option2 != null && option2.WasPressedThisFrame())
        {
            SelectCardForQueue(1);
        }

        if (option3 != null && option3.WasPressedThisFrame())
        {
            SelectCardForQueue(2);
        }

        if (option4 != null && option4.WasPressedThisFrame())
        {
            SelectCardForQueue(3);
        }

        if (option5 != null && option5.WasPressedThisFrame())
        {
            SelectCardForQueue(4);
        }

        if (turnEnd != null && turnEnd.WasPressedThisFrame())
        {
            EndTurn();
        }
    }

    private void ToggleFighterInput(bool isActive)
    {
        if (CurrentState == PlayerState.KO)
        {
            return;
        }

        if (isAIControlled)
        {
            if (isActive && CurrentState == PlayerState.Idle)
            {
                StartAITurn();
            }
            else
            {
                StopAITurn();
            }

            return;
        }

        if (moveSelectMap == null)
        {
            return;
        }

        if (isActive)
        {
            moveSelectMap.Enable();
        }
        else
        {
            moveSelectMap.Disable();
        }
    }

    #region AI

    private void StartAITurn()
    {
        if (!isAIControlled || CurrentState != PlayerState.Idle)
        {
            return;
        }

        StopAITurn();
        aiTurnRoutine = StartCoroutine(PerformAITurn());
    }

    private void StopAITurn()
    {
        if (aiTurnRoutine == null)
        {
            return;
        }

        StopCoroutine(aiTurnRoutine);
        aiTurnRoutine = null;
    }

    private IEnumerator PerformAITurn()
    {
        yield return new WaitForSeconds(aiThinkDelay);

        if (CurrentState != PlayerState.Idle)
        {
            aiTurnRoutine = null;
            yield break;
        }

        ChooseAICards();

        if (CurrentState == PlayerState.Idle)
        {
            EndTurn();
        }

        aiTurnRoutine = null;
    }

    private void ChooseAICards()
    {
        if (hand == null || hand.Count == 0)
        {
            return;
        }

        int minimumCards = Mathf.Clamp(
            aiMinimumCardsToPlay,
            0,
            playerMaxCards);

        int maximumCards = Mathf.Clamp(
            aiMaximumCardsToPlay,
            minimumCards,
            playerMaxCards);

        int cardsToPlay = UnityEngine.Random.Range(
            minimumCards,
            maximumCards + 1);

        List<int> availableIndices = new List<int>();

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i] != null &&
                hand[i]._card != null &&
                !hand[i]._isSelected)
            {
                availableIndices.Add(i);
            }
        }

        int cardsPlayed = 0;

        while (availableIndices.Count > 0 && cardsPlayed < cardsToPlay)
        {
            int availableListIndex = UnityEngine.Random.Range(
                0,
                availableIndices.Count);

            int handIndex = availableIndices[availableListIndex];

            int cardsBeforeSelection = QueuedCards.Count;
            SelectCardForQueue(handIndex);

            if (QueuedCards.Count > cardsBeforeSelection)
            {
                cardsPlayed++;
            }

            // Remove it whether it was affordable or not,
            // so the AI cannot get stuck trying the same card.
            availableIndices.RemoveAt(availableListIndex);
        }

        Debug.Log(
            $"{name} AI queued {QueuedCards.Count} card(s). " +
            $"Mana remaining: {currentMana}.");
    }

    #endregion

    private void AutoSetQueue()
    {
        if (CurrentState == PlayerState.Idle)
        {
            EndTurn();
        }
    }

    internal void DrawForTurn()
    {
        while (Hand.Count < playerMaxCards)
        {
            FighterActions drawnCard = DeckSystem.Instance.Draw();

            if (drawnCard == null)
            {
                Debug.LogWarning($"{name}: Could not draw because deck and discard pile are empty.");
                break;
            }

            Hand.Add(new PlayerCardInHand(drawnCard, false));
        }

        CurrentState = PlayerState.Idle;
        currentMana = maxMana;
        QueuedCards.Clear();

        fighterUI?.UpdateStamina(currentMana);

        handContainerUI?.PopulateHandUI(Hand);
        handContainerUI?.UpdateCardsInUseForTurn();
    }

    private void SelectCardForQueue(int index)
    {
        if (CurrentState != PlayerState.Idle)
        {
            return;
        }

        if (index < 0 || index >= hand.Count)
        {
            return;
        }

        PlayerCardInHand handCard = hand[index];

        if (handCard == null || handCard._card == null)
        {
            return;
        }

        if (handCard._isSelected)
        {
            return;
        }

        FighterActions selectedCard = handCard._card;

        if (selectedCard._ManaCost > currentMana)
        {
            return;
        }

        handCard._isSelected = true;
        currentMana -= selectedCard._ManaCost;
        QueuedCards.Add(selectedCard);

        fighterUI?.UpdateStamina(currentMana);
        handContainerUI?.UpdateCardsInUseForTurn();
    }

    public void EndTurn()
    {
        if (CurrentState != PlayerState.Idle)
        {
            return;
        }

        CurrentState = PlayerState.TurnEnd;

        if (battleContainerUI != null)
        {
            battleContainerUI.AddQueuedCardsToUI(QueuedCards);
        }

        OnPlayerTurnSet?.Invoke();
    }

    internal void DiscardHand()
    {
        if (DeckSystem.Instance == null)
        {
            Debug.LogError($"{name}: DeckSystem.Instance is missing.");
            return;
        }

        foreach (PlayerCardInHand card in hand)
        {
            if (card != null && card._card != null)
            {
                DeckSystem.Instance.DiscardCard(card._card);
            }
        }

        hand.Clear();
        QueuedCards.Clear();

        handContainerUI?.ClearDisplayedCards();
        battleContainerUI?.ClearQueue();
    }

    public void ProcessBattleOutcome(ActionData pOutcome)
    {
        if (CurrentState == PlayerState.KO)
        {
            return;
        }

        currentHP -= pOutcome.Damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        fighterUI?.UpdateHealth(currentHP);

        CheckForDeath();
    }

    private void CheckForDeath()
    {
        if (currentHP > 0 || CurrentState == PlayerState.KO)
        {
            return;
        }

        CurrentState = PlayerState.KO;
        OnPlayerKO?.Invoke();
    }
    
}

[Serializable]
public class PlayerCardInHand
{
    [SerializeField] internal FighterActions _card;
    [SerializeField] internal bool _isSelected;

    public PlayerCardInHand(FighterActions card, bool isSelected)
    {
        _card = card;
        _isSelected = isSelected;
    }
}