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

    Animator animator;

    [SerializeField] float moveDistance = 1.0f;
    [SerializeField] float moveDuration = 0.4f;

    bool isMoving;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

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
        
        QueueEngage();
        animationQueue.Add(Kick);
        animationQueue.Add(Punch);

        animationQueue.Add(Block);
        animationQueue.Add(Dodge);

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



    public void Kick()
    {
        animator.SetTrigger("Kick");
    }

    public void Punch()
    {
        animator.SetTrigger("Punch");
    }

    public void Block()
    {
        animator.SetTrigger("Block");
    }

    public void  Dodge()
    {
        animator.SetTrigger("Dodge");
    }

    IEnumerator Engage()
    {
        isMoving = true;
        animator.SetTrigger("MoveForward");
        yield return Move(transform.forward);
        isMoving = false;
    }

    IEnumerator Disengage()
    {
        isMoving = true;
        animator.SetTrigger("MoveBack");
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
