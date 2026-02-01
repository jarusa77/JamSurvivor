using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Android.Gradle;

public class PlayerAvatarControl : MonoBehaviour
{
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
        if (isMoving) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(Engage());
        }

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            StartCoroutine(Disengage());
        }

        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            Kick();
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            Punch();
        }

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            Block();
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            Dodge();
        }

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
