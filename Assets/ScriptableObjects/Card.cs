using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public enum CardType
{
    Attack, Move, Special, Block
};

[CreateAssetMenu(fileName = "Cards", menuName = "Scriptable Objects/Cards")]
public class Card : ScriptableObject
{
    [SerializeField, HideInInspector] private string uniqueID;

    public string UniqueID => uniqueID;
    
    public string _Name;
    public int _ManaCost;
    
    public CardType _CardType;
    public AttackData _Attack;

    public string _FlavorText;
    public Sprite _TypeSprite;
    
    public MovementResult _MovementResult;
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = GUID.Generate().ToString();
            EditorUtility.SetDirty(this);
        }
    }
    #endif

}

[System.Serializable]
public struct AttackData
{
    public int _Damage;
    public StatusData _Status;
    public float _StartFrames;
    public BattleResult _OnHit;
    public BattleResult _OnMiss;
}

[System.Serializable]
public struct StatusData
{
    public StatusInflict _Status;
    public int _Turns;
}

public enum StatusInflict
{
    None, Stun
}
[System.Serializable]
public struct BattleResult
{
    public int _SelfDelayFrames;
    public int _TargetDelayFrames;
}

[System.Serializable]
public struct MovementResult
{
    public int _BlocksToMove;
    public MoveType _MoveType;
}

public enum MoveType
{
    None, Up, Forward, Back
}
