using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal enum Game_State
{
    Loading,
    PlayerTurn,
    TurnExecute,
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
    }

    private void OnDestroy()
    {
        Fighter.OnPlayerTurnSet -= PlayerConfirmTurnEnd;
    }

    public void PlayerConfirmTurnEnd()
    {
        Debug.Log("A Player Ended their turn");
        //Check for all fighters states if they both ended their turn.
        //Alternatively, just keep a local game manager varialbe, but then if a player wants to change their opinion after lock in, would need to recall event.
        if (_fighters.Any(x => x.CurrentState != PlayerState.TurnEnd))
            return;
        GameState = Game_State.TurnExecute;
        SendDataToTurnExecuteSystem();
    }

    private void SendDataToTurnExecuteSystem()
    {
        Debug.Log("Ready to Execute Battle");
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
