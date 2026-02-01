using UnityEngine;

public class SpotlightFollowPlayer : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Rotation Settings")]
    public bool smoothFollow = true;
    public float rotationSpeed = 5f;

    void Update()
    {
        if (player == null) return;

        // Direction from light to player
        Vector3 direction = player.position - transform.position;

        // Calculate rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (smoothFollow)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }
}
