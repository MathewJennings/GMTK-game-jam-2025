using UnityEngine;

/// <summary>
/// Simple component to mark objects that can break circles.
/// Attach this to any GameObject that should break circles when touched.
/// Make sure the GameObject has a Collider2D component and is on the correct layer.
/// </summary>
public class CircleBreaker : MonoBehaviour
{
    [Header("Circle Breaking")]
    [Tooltip("Optional effect to spawn when this object breaks a circle")]
    public GameObject onBreakEffect;
    
    [Tooltip("Should this object be destroyed when it breaks a circle?")]
    public bool destroyOnBreak = false;
    
    [Tooltip("Should this object move when it breaks a circle?")]
    public bool bounceOnBreak = true;
    
    [Tooltip("Force applied when bouncing")]
    public float bounceForce = 5f;
    
    private Rigidbody2D rb2d;
    
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        
        // Ensure we have a collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning($"CircleBreaker on {gameObject.name} needs a Collider2D component!");
        }
        else
        {
            // IMPORTANT: Set as trigger for EdgeCollider2D detection to work
            collider.isTrigger = true;
            Debug.Log($"CircleBreaker {gameObject.name} setup - Layer: {gameObject.layer}, IsTrigger: {collider.isTrigger}, ColliderType: {collider.GetType().Name}");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"CircleBreaker {gameObject.name} - OnTriggerEnter2D with {other.gameObject.name}");
        
        // Check if we collided with a circle drawer
        InteractiveCircleDrawer circleDrawer = other.GetComponent<InteractiveCircleDrawer>();
        if (circleDrawer != null)
        {
            Debug.Log($"CircleBreaker detected collision with CircleDrawer!");
            OnCircleBroken();
        }
    }
    
    // Also add OnCollisionEnter2D in case the setup is different
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"CircleBreaker {gameObject.name} - OnCollisionEnter2D with {collision.gameObject.name}");
        
        InteractiveCircleDrawer circleDrawer = collision.gameObject.GetComponent<InteractiveCircleDrawer>();
        if (circleDrawer != null)
        {
            Debug.Log($"CircleBreaker detected collision with CircleDrawer via OnCollisionEnter2D!");
            OnCircleBroken();
        }
    }
    
    void OnCircleBroken()
    {
        // Spawn effect if available
        if (onBreakEffect != null)
        {
            Instantiate(onBreakEffect, transform.position, Quaternion.identity);
        }
        
        // Apply bounce effect
        if (bounceOnBreak && rb2d != null)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            rb2d.AddForce(randomDirection * bounceForce, ForceMode2D.Impulse);
        }
        
        // Destroy if requested
        if (destroyOnBreak)
        {
            Destroy(gameObject, 0.1f); // Small delay to allow effects to spawn
        }
        
        Debug.Log($"{gameObject.name} broke a circle!");
    }
}
