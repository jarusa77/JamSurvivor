using System;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    
    
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
        int index = 0;
        foreach (GameObject obj in HandCardsUI)
        {
            obj.GetComponent<CardUI>().InitializeCardUI(currentPlayerHand[index]._card);
            index++;
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
}
