using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Fighter))]
public class SimpleFighterAI : MonoBehaviour
{
    [SerializeField] private float thinkingTime = 1f;
    [SerializeField] private int maxCardsToPlay = 3;

    private Fighter fighter;
    private Coroutine aiTurnRoutine;

    private void Awake()
    {
        fighter = GetComponent<Fighter>();
    }

    private void OnEnable()
    {
        GameManager.OnToggleFighterInput += CheckForAITurn;
    }

    private void OnDisable()
    {
        GameManager.OnToggleFighterInput -= CheckForAITurn;
    }

    private void CheckForAITurn(bool isActive)
    {
        if (!isActive)
            return;

        if (!fighter.IsAIControlled)
            return;

        if (fighter.CurrentState != PlayerState.Idle)
            return;

        if (aiTurnRoutine != null)
            StopCoroutine(aiTurnRoutine);

        aiTurnRoutine = StartCoroutine(ChooseCardsAndEndTurn());
    }

    private IEnumerator ChooseCardsAndEndTurn()
    {
        yield return new WaitForSeconds(thinkingTime);

        List<int> affordableCardIndexes = new List<int>();

        for (int i = 0; i < fighter.CurrentHand.Count; i++)
        {
            PlayerCardInHand cardInHand = fighter.CurrentHand[i];

            if (cardInHand._card == null)
                continue;

            if (cardInHand._isSelected)
                continue;

            if (cardInHand._card._ManaCost <= fighter.AvailableMana)
            {
                affordableCardIndexes.Add(i);
            }
        }

        ShuffleIndexes(affordableCardIndexes);

        int cardsPlayed = 0;

        foreach (int cardIndex in affordableCardIndexes)
        {
            if (cardsPlayed >= maxCardsToPlay)
                break;

            bool cardQueued = fighter.TryQueueCard(cardIndex);

            if (cardQueued)
            {
                cardsPlayed++;

                yield return new WaitForSeconds(0.25f);
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (fighter.CurrentState == PlayerState.Idle)
        {
            fighter.EndTurn();
        }

        aiTurnRoutine = null;
    }

    private void ShuffleIndexes(List<int> indexes)
    {
        for (int i = indexes.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            int temp = indexes[i];
            indexes[i] = indexes[randomIndex];
            indexes[randomIndex] = temp;
        }
    }
}