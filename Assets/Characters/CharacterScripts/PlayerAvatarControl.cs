using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerAvatarControl : MonoBehaviour
{
    private  List<IEnumerator> animationQueue = new List<IEnumerator>();

    [SerializeField] private Animator[] animator;          // size 2
    [SerializeField] private Transform[] playerTransform;  // size 2

    [SerializeField] private float moveDistance = 1.0f;
    [SerializeField] private float moveDuration = 0.4f;

    private bool isRunningSequence;

    public bool debugOn = false;

    public Fighter player1;
    public Fighter player2;
    bool MatchEnd = false;
    GameManager GM;
   

    private void OnEnable()
    {
        TurnSystem.OnBattleResultsCalculated += RunSequence;
        GM = GameManager.Instance;
       
    }


    public void RunSequence(List<ActionStructCompact> p1, List<ActionStructCompact> p2)
    {
        animationQueue.Clear();
        animationQueue = new List<IEnumerator>();
        //QueueEngage();

        Debug.Log("P1 count" + p1.Count);
        Debug.Log("P2 count" + p2.Count);

        for (int i = 0; i < p1.Count; i++)
        {
            Debug.Log("P1 Action: " + p1[i].actionType);
            switch (p1[i].actionType)
            {
                case ActionType.Punch:
                    QueueTrigger("Punch", 0);
                    break;
                case ActionType.Kick:
                    QueueTrigger("Kick", 0);
                    break;
                case ActionType.Block:
                    QueueTrigger("Block", 0);
                    break;
                case ActionType.Feint:
                    QueueTrigger("Dodge", 0);
                    break;
                default: //null
                    break;
            }

            Debug.Log("P2 Action: " + p2[i].actionType);
            switch (p2[i].actionType)
            {
                case ActionType.Punch:
                    QueueTrigger("Punch", 1);
                    break;
                case ActionType.Kick:
                    QueueTrigger("Kick", 1);
                    break;
                case ActionType.Block:
                    QueueTrigger("Block", 1);
                    break;
                case ActionType.Feint:
                    QueueTrigger("Dodge", 1);
                    break;
                default: //null
                    break;
            }

            int p1health = player1.GetHP();
            int p2health = player2.GetHP();

            if (p1health <=0 && p2health <= 0)
            {
                QueueTrigger("Dying", 0);
                QueueTrigger("Dying", 1);
                StartCoroutine(Sequence());
                if(GM  ==  null)
                    GM=GameManager.Instance;
                GM.PlayerGotKO();
                MatchEnd = false;
                return;
            }
            else if(p1health <=0)
            {
                
                QueueTrigger("Dying", 0);
                QueueTrigger("Victory", 1);
                StartCoroutine(Sequence());
                if (GM == null)
                    GM = GameManager.Instance;
                GM.PlayerGotKO();
                MatchEnd = false;
                return;
            }
            else if(p2health <=0)
            {
                QueueTrigger("Victory", 0);
                QueueTrigger("Dying", 1);
                StartCoroutine(Sequence());
                if (GM == null)
                    GM = GameManager.Instance;
                GM.PlayerGotKO();
                MatchEnd = false;
                return;
            }


        }

        StartCoroutine(EngageBoth());

    }

    IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(5f);
        GM.TriggerGameEnd();
    }


    IEnumerator Sequence()
    {
        isRunningSequence = true;
        

        for (int i = 0; i < animationQueue.Count; i++)
        {
            // IMPORTANT: don't StartCoroutine() a null
            if (animationQueue[i] != null)
                yield return StartCoroutine(animationQueue[i]);
        }
      

        animationQueue.Clear();
        isRunningSequence = false;

        if(MatchEnd)
            StartCoroutine(EndDelay());
        else
            StartCoroutine(DisengageBoth());
    }

    void QueueTrigger(string trigger, int playerIndex)
    {
        animationQueue.Add(TriggerRoutine(trigger, playerIndex));
    }

    IEnumerator TriggerRoutine(string trigger, int playerIndex)
    {
        animator[playerIndex].SetTrigger(trigger);
        yield break; // instant step
    }

    IEnumerator WaitRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    void QueueEngage() => animationQueue.Add(EngageBoth());
    void QueueDisengage() => animationQueue.Add(DisengageBoth());


    IEnumerator EngageBoth()
    {
        animator[0].SetTrigger("MoveForward");
        animator[1].SetTrigger("MoveForward");

        yield return MoveBoth(
            playerTransform[0], playerTransform[0].forward,
            playerTransform[1], playerTransform[1].forward
        );

        StartCoroutine(Sequence());
    }

    IEnumerator DisengageBoth()
    {
        animator[0].SetTrigger("MoveBack");
        animator[1].SetTrigger("MoveBack");

        yield return MoveBoth(
            playerTransform[0], -playerTransform[0].forward,
            playerTransform[1], -playerTransform[1].forward
        );
    }

    IEnumerator MoveBoth(Transform a, Vector3 dirA, Transform b, Vector3 dirB)
    {
        Vector3 startA = a.position;
        Vector3 endA = startA + dirA.normalized * moveDistance;

        Vector3 startB = b.position;
        Vector3 endB = startB + dirB.normalized * moveDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            a.position = Vector3.Lerp(startA, endA,  t);
            b.position = Vector3.Lerp(startB, endB, t);
            yield return null;
        }
    }
}
