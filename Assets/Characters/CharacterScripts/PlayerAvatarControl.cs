using UnityEngine;

public class PlayerAvatarControl : MonoBehaviour
{
    
    Animator animator;

    float EngageDistance = 1.0f;

    public void engage()
    {
        transform.Translate(Vector3.forward );
        animator.SetTrigger("MoveForward");
    }


}
