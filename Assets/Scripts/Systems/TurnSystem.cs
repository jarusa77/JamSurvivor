using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance;
    [SerializeField] private FighterActions NoAction;

    public delegate void BattleResultsCalculated(List<ActionStructCompact>p1, List<ActionStructCompact>p2);
    public static event BattleResultsCalculated OnBattleResultsCalculated;

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
        List<ActionStructCompact> p1 = new List<ActionStructCompact>();
        List<ActionStructCompact> p2 = new  List<ActionStructCompact>();
        
        int MaxTurncount = System.Math.Max(fighter1.QueuedCards.Count, fighter2.QueuedCards.Count);
        for (int turnCount = 0; turnCount < MaxTurncount; turnCount++)
        {
            ActionStructCompact p1Struct = new  ActionStructCompact();
            ActionStructCompact p2Struct = new  ActionStructCompact();
            ActionData player1Outcome = new ActionData();
            ActionData player2Outcome = new ActionData();
            
            
            FighterActions p1CardToPlay =  fighter1.QueuedCards.Count > turnCount ? fighter1.QueuedCards[turnCount] : NoAction;
            FighterActions p2CardToPlay = fighter2.QueuedCards.Count > turnCount ? fighter2.QueuedCards[turnCount] : NoAction;
            
            
            player1Outcome = ResolutionSystem.ResolveForCards(p1CardToPlay, p2CardToPlay);
            player2Outcome = ResolutionSystem.ResolveForCards(p2CardToPlay, p1CardToPlay);
            
            p1Struct.actionData = player1Outcome;
            p1Struct.actionType = p1CardToPlay._ActionType;
            p2Struct.actionData = player2Outcome;
            p2Struct.actionType = p2CardToPlay._ActionType;
            
            p1.Add(p1Struct);
            p2.Add(p2Struct);
            //TODO: if animation is going to be calling the damage, then remove this.
            fighter1.ProcessBattleOutcome(player2Outcome);
            fighter2.ProcessBattleOutcome(player1Outcome);
        }
        OnBattleResultsCalculated?.Invoke(p1, p2);
    }
}

public struct ActionStructCompact
{
    public ActionType actionType;
    public ActionData actionData;
}
