using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float lookAhead = 6f;
    public float smoothTime = 0.2f;
    public bool followY = true;
    public float yOffset = 1f;

    private Vector3 velocity;
    private float fixedZ;

    private void Awake()
    {
        fixedZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = transform.position;
        targetPosition.x = target.position.x + lookAhead;
        targetPosition.y = followY ? target.position.y + yOffset : transform.position.y;
        targetPosition.z = fixedZ;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
