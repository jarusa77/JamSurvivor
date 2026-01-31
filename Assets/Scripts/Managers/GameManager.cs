using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal enum Game_State
{
    Loading,
    PlayerTurn,
    TurnExecute,
    KOEvaluation,
    GameOver,
    Pause
}
public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    internal Game_State GameState = Game_State.Loading;

    private List<Fighter> _fighters;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        _fighters = new List<Fighter>();
        GameState = Game_State.Loading;
        Fighter.OnPlayerTurnSet += PlayerConfirmTurnEnd;
        Fighter.OnPlayerKO += PlayerGotKO;
    }

    private void PlayerGotKO()
    {
        GameState =  Game_State.KOEvaluation;
    }

    private void OnDestroy()
    {
        Fighter.OnPlayerTurnSet -= PlayerConfirmTurnEnd;
        Fighter.OnPlayerKO -= PlayerGotKO;
    }

    public void PlayerConfirmTurnEnd()
    {
        //Check for all fighters states if they both ended their turn.
        //Alternatively, just keep a local game manager varialbe, but then if a player wants to change their opinion after lock in, would need to recall event.
        if (_fighters.Any(x => x.CurrentState != PlayerState.TurnEnd))
            return;
        GameState = Game_State.TurnExecute;
        SendDataToTurnExecuteSystem();
    }

    private void SendDataToTurnExecuteSystem()
    {
        TurnSystem.Instance.ExecuteBattle(_fighters[0], _fighters[1]);
        BattleEnded();
    }

    public void BattleEnded()
    {
        SetupNextPlayerTurn();
    }

    public void SetupNextPlayerTurn()
    {
        foreach (Fighter fighter in _fighters)
        {
            fighter.DiscardHand();
        }
        SetPlayersHand();
    }

    public void AddFighter(Fighter fighter)
    {
        _fighters.Add(fighter);
        if (_fighters.Count >= 2)
        {
            GameState = Game_State.PlayerTurn;
            SetPlayersHand();
        }
    }

    private void SetPlayersHand()
    {
        foreach (Fighter fighter in _fighters)
        {
            fighter.DrawForTurn();
        }
    }
}
