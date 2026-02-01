using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Android.Gradle;
using NUnit.Framework;
using System.Collections.Generic;




public class PlayerAvatarControl : MonoBehaviour
{
    delegate void AnimationAction();

    List<AnimationAction> animationQueue = new List<AnimationAction>();
    [SerializeField]
    Animator[] animator;
    [SerializeField]
    Transform[] PlayerTransform;

    [SerializeField] float moveDistance = 1.0f;
    [SerializeField] float moveDuration = 0.4f;

    bool isMoving;


    void Update()
    {
        test();

    }

    public void test()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            RunSequence();
        }

    }

    public void SetSequence()
    {
        /*
         * Punch
         * Kick
         * Block
         * MoveBack
         * MoveForward
         * Dodge
         */
        
        QueueEngage();
        SetTrigger("Kick", 0);
        SetTrigger("Block", 1);

        SetTrigger("Block", 0);
        SetTrigger("Punch", 1);

        SetTrigger("Kick", 0);
        SetTrigger("Dodge", 1);


        QueueDisengage();
    }

    public void RunSequence()
    {
        SetSequence();
       StartCoroutine (Sequence());

        

    }

    IEnumerator Sequence()
    {
        for (int i = 0; i < animationQueue.Count; i++)
        {
            animationQueue[i]?.Invoke();
            if(i % 2==1)
                yield return new WaitForSeconds(1f);
        }
        animationQueue.Clear();
    }


        void QueueEngage()
    {
        animationQueue.Add(() => StartCoroutine(Engage()));
    }

    void QueueDisengage()
    {
        animationQueue.Add(() => StartCoroutine(Disengage()));
    }



    public void SetTrigger(string Trigger,int PlayerIndex)
    {
        animationQueue.Add(() => animator[PlayerIndex].SetTrigger(Trigger));
    }

  

    IEnumerator Engage()
    {
        isMoving = true;
        animator[0].SetTrigger("MoveForward");
        animator[1].SetTrigger("MoveForward");
        yield return Move(transform.forward);
        isMoving = false;
    }

    IEnumerator Disengage()
    {
        isMoving = true;
        animator[0].SetTrigger("MoveBack");
        animator[1].SetTrigger("MoveForward");

        yield return Move(-transform.forward);
        isMoving = false;
    }

    IEnumerator Move(Vector3 direction)
    {
        Vector3 start = transform.position;
        Vector3 end = start + direction.normalized * moveDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }
}
