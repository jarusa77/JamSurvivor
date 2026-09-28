using System;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField] private Color CardSelected;
    [SerializeField] private Color DefaultColor;
    private List<PlayerCardInHand> CurrentPlayerHand;

    [SerializeField] private GameObject CardUIPrefab;
    private List<GameObject> HandCardsUI;

    internal List<FighterActions> _FighterActions;

    private void Awake()
    {
        HandCardsUI = new List<GameObject>();
    }
    public void CreatePlaceholders(int totalMaxCardsOnHand)
    {
        for (int i = 0; i < totalMaxCardsOnHand; i++)
        {
            GameObject card = Instantiate(CardUIPrefab, this.transform);
            HandCardsUI.Add(card);
        }
    }

    public void PopulateHandUI(List<PlayerCardInHand> currentPlayerHand)
    {
        if (currentPlayerHand == null)
        {
            Debug.LogWarning("PopulateHandUI received a null hand.");
            return;
        }

        for (int i = 0; i < HandCardsUI.Count; i++)
        {
            GameObject cardObject = HandCardsUI[i];

            if (cardObject == null)
            {
                continue;
            }

            bool hasValidCard =
                i < currentPlayerHand.Count &&
                currentPlayerHand[i] != null &&
                currentPlayerHand[i]._card != null;

            cardObject.SetActive(hasValidCard);

            if (!hasValidCard)
            {
                continue;
            }

            CardUI cardUI = cardObject.GetComponent<CardUI>();

            if (cardUI == null)
            {
                Debug.LogError(
                    $"{cardObject.name}: This hand-card prefab is missing its CardUI component.");

                continue;
            }

            cardUI.InitializeCardUI(currentPlayerHand[i]._card);
        }
    }
    public void UpdateCardsInUseForTurn()
    {
        if (CurrentPlayerHand == null)
        {
            return;
        }

        for (int i = 0; i < HandCardsUI.Count; i++)
        {
            if (HandCardsUI[i] == null)
            {
                continue;
            }

            CardUI cardUI = HandCardsUI[i].GetComponent<CardUI>();

            if (cardUI == null)
            {
                continue;
            }

            bool isSelected =
                i < CurrentPlayerHand.Count &&
                CurrentPlayerHand[i] != null &&
                CurrentPlayerHand[i]._isSelected;

            cardUI.SetCardColor(isSelected ? CardSelected : DefaultColor);
        }
    }

    public void populateHandUI(List<PlayerCardInHand> currentPlayerHand)
    {
        if (currentPlayerHand == null)
        {
            Debug.LogWarning("PopulateHandUI received a null hand.");
            return;
        }

        for (int i = 0; i < HandCardsUI.Count; i++)
        {
            GameObject cardObject = HandCardsUI[i];

            if (cardObject == null)
            {
                continue;
            }

            bool hasValidCard =
                i < currentPlayerHand.Count &&
                currentPlayerHand[i] != null &&
                currentPlayerHand[i]._card != null;

            cardObject.SetActive(hasValidCard);

            if (!hasValidCard)
            {
                continue;
            }

            CardUI cardUI = cardObject.GetComponent<CardUI>();

            if (cardUI == null)
            {
                Debug.LogError(
                    $"{cardObject.name}: This hand-card prefab is missing its CardUI component.");

                continue;
            }

            cardUI.InitializeCardUI(currentPlayerHand[i]._card);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClearDisplayedCards()
    {
        foreach (GameObject cardUIObject in HandCardsUI)
        {
            if (cardUIObject != null)
            {
                cardUIObject.SetActive(false);
            }
        }
    }
}
