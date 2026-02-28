using System.Collections.Generic;
using UnityEngine;

public class BattleCardUI : MonoBehaviour
{
    [SerializeField] GameObject CardUIPrefab;

    //Call only after Turn is done and before Draw Card
    public void ClearQueue()
    {
        //TODO: probably cleaner way to do this
        foreach(Transform child in  transform)
            Destroy(child.gameObject);
    }

    //For now adding all cards in bulk - adding it once player confirms turn
    public void AddQueuedCardsToUI(List<FighterActions> pActions, int PlayerID)
    {
        int Index = 0;
        foreach (FighterActions pAction in pActions)
        {
            var gameobject = Instantiate(CardUIPrefab, this.transform);
            gameobject.GetComponent<CardUI>().InitializeCardUI(pAction, PlayerID, Index);
        }
    }

    void Awake()
    {
        ClearQueue();
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
