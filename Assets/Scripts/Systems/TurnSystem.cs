using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance;
    [SerializeField] private FighterActions NoAction;

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
        //TODO: Storing actions as we might want to return them for animations later; these will include any NoActions that qualify during the battle.
        List<FighterActions> p1 = new List<FighterActions>();
        List<FighterActions> p2 = new  List<FighterActions>();
        
        int MaxTurncount = System.Math.Max(fighter1.QueuedCards.Count, fighter2.QueuedCards.Count);
        for (int turnCount = 0; turnCount < MaxTurncount; turnCount++)
        {
            ActionData player1Outcome = new ActionData();
            ActionData player2Outcome = new ActionData();
            
            
            FighterActions p1CardToPlay =  fighter1.QueuedCards.Count > turnCount ? fighter1.QueuedCards[turnCount] : NoAction;
            FighterActions p2CardToPlay = fighter2.QueuedCards.Count > turnCount ? fighter2.QueuedCards[turnCount] : NoAction;
            
            p1.Add(p1CardToPlay);
            p2.Add(p2CardToPlay);
            
            player1Outcome = ResolutionSystem.ResolveForCards(p1CardToPlay, p2CardToPlay);
            player2Outcome = ResolutionSystem.ResolveForCards(p2CardToPlay, p1CardToPlay);
            
            fighter1.ProcessBattleOutcome(player2Outcome);
            fighter2.ProcessBattleOutcome(player1Outcome);
        }
    }
}
