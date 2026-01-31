using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "FighterActions", menuName = "Scriptable Objects/FighterActions")]
public class FighterActions : ScriptableObject
{
    [SerializeField, HideInInspector] private string uniqueID;
    public string UniqueID => uniqueID;
    public string Name;
    public int _ManaCost;
    
    public ActionType _ActionType;

    public ActionData Full;
    public ActionData Partial;
    public ActionData Miss;
    
    
    
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

public enum ActionType
{
    Punch = 0, 
    Kick = 1, 
    Block = 2, 
    Feint = 3,
    None = 4
}

[System.Serializable]
public struct ActionData
{
    public int Damage;
    public StatusData Status;
}
