using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ExecuteBattle(Fighter fighter1, Fighter fighter2)
    {
        int MaxTurncount = System.Math.Max(fighter1.QueuedCards.Count, fighter2.QueuedCards.Count);
        for (int turnCount = 0; turnCount < MaxTurncount; turnCount++)
        {
            AttackOutcome player1Outcome = new AttackOutcome();
            AttackOutcome player2Outcome = new AttackOutcome();
            
            
            Card p1CardToPlay =  fighter1.QueuedCards.Count >= turnCount ? fighter2.QueuedCards[turnCount] : null;
            Card p2CardToPlay = fighter2.QueuedCards.Count >= turnCount ? fighter2.QueuedCards[turnCount] : null;
            
            player1Outcome = ResolutionSystem.ResolveForCards(p1CardToPlay, p2CardToPlay);
            player2Outcome = ResolutionSystem.ResolveForCards(p2CardToPlay, p1CardToPlay);
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
