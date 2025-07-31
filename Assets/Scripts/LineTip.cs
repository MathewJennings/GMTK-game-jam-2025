using UnityEngine;

/// <summary>
/// Represents the tip/cursor of the line being drawn. 
/// This object moves with the mouse and can detect collisions with CircleBreaker objects.
/// </summary>
public class LineTip : MonoBehaviour
{
    [Header("Line Tip Settings")]
    [Tooltip("Radius of the collision detection")]
    public float collisionRadius = 0.1f;
    
    [Tooltip("Layer mask for objects that can break the circle")]
    public LayerMask breakingObjectsLayer = -1;
    
    [Tooltip("Visual representation of the tip (optional)")]
    public GameObject tipVisual;
    
    private CircleCollider2D tipCollider;
    private InteractiveCircleDrawer circleDrawer;
    private bool isActive = false;
    
    void Awake()
    {
        // Get reference to the circle drawer
        circleDrawer = GetComponentInParent<InteractiveCircleDrawer>();
        if (circleDrawer == null)
        {
            circleDrawer = FindObjectOfType<InteractiveCircleDrawer>();
        }
        
        // Setup collider
        tipCollider = GetComponent<CircleCollider2D>();
        if (tipCollider == null)
        {
            tipCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        
        tipCollider.isTrigger = true;
        tipCollider.radius = collisionRadius;
        
        // Hide initially
        SetActive(false);
        
        Debug.Log($"LineTip initialized - Layer: {gameObject.layer}");
    }
    
    public void SetActive(bool active)
    {
        isActive = active;
        gameObject.SetActive(active);
        
        if (tipVisual != null)
        {
            tipVisual.SetActive(active);
        }
        
        Debug.Log($"LineTip set active: {active}");
    }
    
    public void UpdatePosition(Vector3 newPosition)
    {
        if (isActive)
        {
            transform.position = newPosition;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;
        
        Debug.Log($"LineTip collision detected with: {other.gameObject.name} on layer {other.gameObject.layer}");
        
        // Check if this object can break circles
        if (IsBreakingObject(other))
        {
            Debug.Log($"LineTip detected collision with CircleBreaker: {other.gameObject.name}");
            
            // Notify the circle drawer that we hit something
            if (circleDrawer != null)
            {
                circleDrawer.OnLineTipCollision(other.transform.position);
            }
        }
        else
        {
            Debug.Log($"LineTip collision ignored - not a breaking object");
        }
    }
    
    bool IsBreakingObject(Collider2D other)
    {
        // Check if the object is on a layer that can break the circle
        bool canBreak = ((1 << other.gameObject.layer) & breakingObjectsLayer) != 0;
        
        // Also check if it has a CircleBreaker component
        bool hasCircleBreaker = other.GetComponent<CircleBreaker>() != null;
        
        Debug.Log($"LineTip checking if {other.gameObject.name} (layer {other.gameObject.layer}) can break circle. LayerMask: {breakingObjectsLayer.value}, HasCircleBreaker: {hasCircleBreaker}, Result: {canBreak && hasCircleBreaker}");
        
        return canBreak && hasCircleBreaker;
    }
    
    void OnValidate()
    {
        if (tipCollider != null)
        {
            tipCollider.radius = collisionRadius;
        }
    }
}
