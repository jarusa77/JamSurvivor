using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    
    [SerializeField] private EndGameCreditsLoader _EndGameCreditsLoader;

    public delegate void ToggleFighterInput(bool isActive);
    public static event ToggleFighterInput OnToggleFighterInput;

    private List<Fighter> _fighters;

    private int RoundCount = 1;
    [SerializeField] private TextMeshProUGUI RoundText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _fighters = new List<Fighter>();
        GameState = Game_State.Loading;
        Fighter.OnPlayerTurnSet += PlayerConfirmTurnEnd;
        Fighter.OnPlayerKO += PlayerGotKO;
        TurnSystem.OnBattleResultsCalculated += GetBattleResults;
    }

    public void TriggerGameEnd()
    {
        Timer.Instance.StopTimer();
        OnToggleFighterInput?.Invoke(false);
        _EndGameCreditsLoader.EndGame();
    }
    
    

    private void GetBattleResults(List<ActionStructCompact> p1, List<ActionStructCompact> p2)
    {
        Debug.Log("Successfull message recieved");
    }

    public void PlayerGotKO()
    {
        GameState =  Game_State.KOEvaluation;
        Timer.Instance.StopTimer();
        OnToggleFighterInput?.Invoke(false);
    }

    private void OnDestroy()
    {
        Fighter.OnPlayerTurnSet -= PlayerConfirmTurnEnd;
        Fighter.OnPlayerKO -= PlayerGotKO;
        TurnSystem.OnBattleResultsCalculated -= GetBattleResults;
    }

    public void PlayerConfirmTurnEnd()
    {
        //Check for all fighters states if they both ended their turn.
        //Alternatively, just keep a local game manager varialbe, but then if a player wants to change their opinion after lock in, would need to recall event.
        if (_fighters.Any(x => x.CurrentState != PlayerState.TurnEnd))
            return;
        GameState = Game_State.TurnExecute;
        OnToggleFighterInput?.Invoke(false);
        SendDataToTurnExecuteSystem();
    }

    private void SendDataToTurnExecuteSystem()
    {
        TurnSystem.Instance.ExecuteBattle(_fighters[0], _fighters[1]);
        BattleEnded();
    }

    public void BattleEnded()
    {
        Debug.Log("Fighter ID: " + _fighters[0].GetID() +
                  "Fighter HP: "+_fighters[0].GetHP());
        Debug.Log("Fighter ID: " + _fighters[1].GetID() +
                  "Fighter HP: "+_fighters[1].GetHP());
        SetupNextPlayerTurn();
    }

    public void SetupNextPlayerTurn()
    {
        foreach (Fighter fighter in _fighters)
        {
            fighter.DiscardHand();
        }
        SetPlayersHand();
        OnToggleFighterInput?.Invoke(true);
        RoundCount++;
        RoundText.text = RoundCount.ToString();
        Timer.Instance.FightBegin();
    }

    public void AddFighter(Fighter fighter)
    {
        _fighters.Add(fighter);
        if (_fighters.Count >= 2)
        {
            //GAME READY TO START
            GameState = Game_State.PlayerTurn;
            SetPlayersHand();
            IntroAnimationPlay();
        }
    }

    private void SetPlayersHand()
    {
        foreach (Fighter fighter in _fighters)
        {
            fighter.DrawForTurn();
        }
        //Probably not needed right now
        //OnToggleFighterInput?.Invoke(false);
        
    }

    private void IntroAnimationPlay()
    {
        //TODO: if playing some intro animation, let it ride until it's done, then start the timer and enable the player inputs
        Timer.Instance.FightBegin();
        OnToggleFighterInput?.Invoke(true);
    }
}
