using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerAvatarControl : MonoBehaviour
{
    private readonly List<IEnumerator> animationQueue = new();

    [SerializeField] private Animator[] animator;          // size 2
    [SerializeField] private Transform[] playerTransform;  // size 2

    [SerializeField] private float moveDistance = 1.0f;
    [SerializeField] private float moveDuration = 0.4f;

    private bool isRunningSequence;

    private void OnEnable()
    {
        TurnSystem.OnBattleResultsCalculated += RunSequence;
    }

    private void Update()
    {
        test();
    }

    public void test()
    {
        //if (isMoving) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(EngageBoth());
        }

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            StartCoroutine(DisengageBoth());
        }

        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            animator[0].SetTrigger("Kick");
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            animator[1].SetTrigger("Punch");
        }

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            animator[0].SetTrigger("Block");
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            animator[1].SetTrigger("Dodge");
        }

    }



    public void RunSequence(List<ActionStructCompact> p1, List<ActionStructCompact> p2)
    {
        animationQueue.Clear();
        QueueEngage();

        for (int i = 0; i < p1.Count; i++)
        {
            switch (p1[i].actionType)
            {
                case ActionType.Punch:
                    QueueTrigger("Punch", 0);
                    break;
                case ActionType.Kick:
                    QueueTrigger("Punch", 0);
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

            switch (p2[i].actionType)
            {
                case ActionType.Punch:
                    QueueTrigger("Punch", 1);
                    break;
                case ActionType.Kick:
                    QueueTrigger("Punch", 1);
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
        }
        

        QueueDisengage();

        StartCoroutine(Sequence());

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
