using System;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField] private Color CardSelected;
    [SerializeField] private Color DefaultColor;
    
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
        for (int i = 0; i < HandCardsUI.Count; i++)
        {
            if (i >= currentPlayerHand.Count || currentPlayerHand[i]._card == null)
            {
                HandCardsUI[i].SetActive(false);
                continue;
            }

            HandCardsUI[i].SetActive(true);
            HandCardsUI[i].GetComponent<CardUI>()
                .InitializeCardUI(currentPlayerHand[i]._card);
        }
    }

    public void UpdateCardsInUseForTurn()
    {
        
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
