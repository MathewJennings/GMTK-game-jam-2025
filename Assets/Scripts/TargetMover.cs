using UnityEngine;

public class TargetMover : MonoBehaviour
{
    public Vector2 direction; // Direction of movement
    public float speed;       // Speed of movement

    void Update()
    {
        // Move the object in the specified direction
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
}
