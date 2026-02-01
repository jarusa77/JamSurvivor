using System;
using NUnit.Framework.Constraints;
using UnityEngine;

public enum ActionResult
{
    Full, Partial, Miss
}

public static class ResolutionSystem
{



    public static ActionData ResolveForCards(FighterActions attacker, FighterActions reciever)
    {
        ActionResult[,] resultMatrix =
        {
                        //Punch,               Kick,                   Block,                  Feint,       None               
            /*Punch */
            { ActionResult.Full, ActionResult.Partial, ActionResult.Partial, ActionResult.Miss, ActionResult.Full },
            /*Kick */
            { ActionResult.Full, ActionResult.Partial, ActionResult.Partial, ActionResult.Miss, ActionResult.Full },
            /*Block */
            { ActionResult.Miss, ActionResult.Miss, ActionResult.Miss, ActionResult.Miss, ActionResult.Full },
            /*Feint */
            { ActionResult.Miss, ActionResult.Miss, ActionResult.Miss, ActionResult.Miss, ActionResult.Full },
            /*None */ 
            { ActionResult.Miss, ActionResult.Miss, ActionResult.Miss, ActionResult.Miss, ActionResult.Full },
        };
        ActionResult outcome = resultMatrix[(int)attacker._ActionType, (int)reciever._ActionType];
        AttackOutcome resultingBattle = new AttackOutcome();
        switch (outcome)
        {
            case ActionResult.Full:
                return attacker.Full;
            case ActionResult.Partial:
                return attacker.Partial;
            case ActionResult.Miss:
                return attacker.Miss;
            default:
                Debug.Log("No Action Result Returned");
                return attacker.Miss;
        }
    }
    

    private static AttackOutcome EvaluateAttackOutcome(Card Attacker, Card Reciever)
    {
        AttackOutcome outcome = new AttackOutcome();
        if (!Reciever)
        {
            //successful action by attacker
            outcome._SuccessfullAttack = true;
            outcome._Damage = Attacker._Attack._Damage;
        }

        else if (Attacker)
        {
            switch (Attacker._CardType)
            {
                case CardType.Attack:
                    if (Reciever._CardType == CardType.Block)
                    {
                        outcome._SuccessfullAttack = false;
                        outcome._Damage = 0;
                    }
                    else
                    {
                        outcome._SuccessfullAttack = true;
                        outcome._Damage = Attacker._Attack._Damage;
                    }

                    break;

                case CardType.Block:
                    if (Reciever._CardType == CardType.Attack)
                    {
                        outcome._SuccessfullAttack = true;
                        outcome._Damage = Attacker._Attack._Damage;
                    }

                    break;

                default:
                    outcome._SuccessfullAttack = false;
                    outcome._Damage = 0;
                    break;
            }
        }

        else
        {
            Debug.Log("Not Playing any Action");
            outcome._SuccessfullAttack = false;
            outcome._Damage = 0;
        }

        return outcome;
        }
}


public struct AttackOutcome
{
    internal int _Damage;
    internal bool _SuccessfullAttack;
}
