using System;
using NUnit.Framework.Constraints;
using UnityEngine;

public static class ResolutionSystem
{
    public static AttackOutcome ResolveForCards(Card attacker, Card reciever)
    {
        return SuccessfullAction(attacker, reciever);
    }

    private static AttackOutcome SuccessfullAction(Card attack1, Card attack2)
    {
        AttackOutcome outcome = new AttackOutcome();
        switch (attack1._CardType)
        {
            case CardType.Attack:
                if (attack2._CardType == CardType.Block)
                {
                    outcome._SuccessfullAttack = false;
                    outcome._Damage = 0;
                }
                else
                {
                    outcome._SuccessfullAttack = true;
                    outcome._Damage = attack1._Attack._Damage;
                }
                break;
                
            case CardType.Block:
                if (attack2._CardType == CardType.Attack)
                {
                    outcome._SuccessfullAttack = true;
                    outcome._Damage = attack1._Attack._Damage;
                }

                break;
                
            default:
                outcome._SuccessfullAttack = false;
                outcome._Damage = 0;
                break;
        }

        return outcome;
    }
}


public struct AttackOutcome
{
    internal int _Damage;
    internal bool _SuccessfullAttack;
}
