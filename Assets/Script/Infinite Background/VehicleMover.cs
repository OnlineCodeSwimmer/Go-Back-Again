using UnityEngine;

public class VehicleMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float railY = 0f;

    private void Update()
    {
        Vector3 position = transform.position;
        position.x += moveSpeed * Time.deltaTime;
        position.y = railY;
        transform.position = position;
    }

    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}
