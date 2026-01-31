
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DeckSystem : MonoBehaviour
{
    private  System.Random rng = new System.Random();
    public  List<FighterActions> Discard;
    public  List<FighterActions> Deck;

    public static DeckSystem Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Discard = new List<FighterActions>();
    }

    public static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public FighterActions Draw()
    {
        if (Deck.Count == 0)
        {
            if (Discard.Count > 0)
            {
                Shuffle(Discard);
                Deck.Clear();
                foreach (FighterActions c in Discard)
                {
                    Deck.Add(c);
                }
                Discard.Clear();
            }
            else
            {
                Debug.Log("Nothing in Deck and Nothing in Discard");
                return null;
            }
        }
        FighterActions drewCard = Deck.FirstOrDefault();
        Deck.RemoveAt(0);
        return drewCard;
    }

    public void DiscardCard(FighterActions usedCard)
    {
        Discard.Add(usedCard);
    }
}