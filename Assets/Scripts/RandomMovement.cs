using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This script is used to move an object randomly around the screen.
/// Example Configurations:
///
/// Slowly drifting coin:
/// Move Speed: 1.5
/// Direction Change Interval: 4
/// Movement Range: (3, 3)
/// Use Perlin Noise: true
///
/// Energetic floating object:
/// Move Speed: 3
/// Direction Change Interval: 2
/// Movement Range: (5, 5)
/// Use Perlin Noise: false
/// </summary>
public class RandomMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private float directionChangeInterval = 3f; // Time between direction changes

    [SerializeField]
    private bool constrainMovementRange = true; // Whether to restrict where the random movement can go
    
    [SerializeField]
    private Vector2 movementRange = new Vector2(5f, 5f); // How far from starting position it can move

    [Header("Screen Boundary Settings")]
    [SerializeField]
    private bool constrainToScreen = true;

    [FormerlySerializedAs("screenPadding")] [SerializeField]
    private float screenPaddingInPx = 50f; // Distance from screen edges

    [Header("Smoothness Settings")]
    [SerializeField]
    private float smoothness = 0.5f; // How smooth the direction changes are (0 = instant, 1 = very smooth)

    [SerializeField]
    private bool usePerlinNoise = false; // Use Perlin noise for more organic movement

    [SerializeField]
    private float noiseScale = 0.5f; // Scale for Perlin noise (smaller = smoother)

    private Vector3 startingPosition;
    private Vector2 currentDirection;
    private Vector2 targetDirection;
    private float directionChangeTimer;
    private Camera mainCamera;
    private Vector2 screenBoundsInPx;
    private float noiseOffset;

    void Start()
    {
        startingPosition = transform.position;
        mainCamera = Camera.main;

        if (constrainToScreen && mainCamera != null)
        {
            screenBoundsInPx = new Vector2(Screen.width, Screen.height);
        }
        SetRandomDirection();
        currentDirection = targetDirection;

        // Random offset for Perlin noise to make each object unique
        noiseOffset = Random.Range(0f, 1000f);
    }

    void Update()
    {
        if (usePerlinNoise)
        {
            MoveWithPerlinNoise();
        }
        else
        {
            MoveWithRandomDirection();
        }

        CheckScreenBoundsAndMaybeChangeDirection();
    }

    private void CheckScreenBoundsAndMaybeChangeDirection()
    {
        if (constrainToScreen && mainCamera != null)
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
            float hardPaddingX = screenPaddingInPx / screenBoundsInPx.x;
            float hardPaddingY = screenPaddingInPx / screenBoundsInPx.y;
            
            float softPaddingX = hardPaddingX + 0.15f;
            float softPaddingY = hardPaddingY + 0.15f;

            // If near the screen boundary, bias direction toward the screen center
            if (viewportPos.x < softPaddingX || viewportPos.x > 1f - softPaddingX || viewportPos.y < softPaddingY || viewportPos.y > 1f - softPaddingY)
            {
                Vector2 directionToCenter = (mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, transform.position.z)) - transform.position).normalized;
                // Generate a random angle offset within ±45 degrees
                float randomAngle = Random.Range(-45f, 45f) * Mathf.Deg2Rad;

                // Rotate the directionToCenter vector by the random angle
                float cosAngle = Mathf.Cos(randomAngle);
                float sinAngle = Mathf.Sin(randomAngle);
                Vector2 directionNearCenter = new Vector2(
                    directionToCenter.x * cosAngle - directionToCenter.y * sinAngle,
                    directionToCenter.x * sinAngle + directionToCenter.y * cosAngle
                ).normalized;
                
                targetDirection = Vector2.Lerp(targetDirection, directionNearCenter, 0.2f).normalized;
                directionChangeTimer = 0f;
            }
            
        }
    }

    private void MoveWithRandomDirection()
    {
        directionChangeTimer += Time.deltaTime;
        if (directionChangeTimer >= directionChangeInterval)
        {
            SetRandomDirection();
        }
        currentDirection = Vector2.Lerp(currentDirection, targetDirection, smoothness * Time.deltaTime * 2f);

        Vector3 movement = moveSpeed * Time.deltaTime * currentDirection;
        Vector3 newPosition = transform.position + movement;
        newPosition = ApplyConstraints(newPosition);
        transform.position = newPosition;
    }

    private void MoveWithPerlinNoise()
    {
        float time = Time.time * noiseScale + noiseOffset;
        float x = Mathf.PerlinNoise(time, 0f) * 2f - 1f; // Convert from 0-1 to -1-1
        float y = Mathf.PerlinNoise(0f, time) * 2f - 1f;

        Vector2 noiseDirection = new Vector2(x, y).normalized;
        Vector3 movement = moveSpeed * Time.deltaTime * noiseDirection;
        Vector3 newPosition = transform.position + movement;
        newPosition = ApplyConstraints(newPosition);
        transform.position = newPosition;
    }

    private void SetRandomDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        targetDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        
        // Ensure the angle between currentDirection and targetDirection is <= 90 degrees
        if (Vector2.Dot(currentDirection, targetDirection) < 0)
        {
            targetDirection = -targetDirection; // Flip the direction to make it closer
        }

        // If we're near the movement range boundary, bias direction toward center
        Vector3 currentOffset = transform.position - startingPosition;
        if (constrainMovementRange && currentOffset.magnitude > movementRange.magnitude * 0.7f)
        {
            Vector2 directionToCenter = (startingPosition - transform.position).normalized;
            targetDirection = Vector2.Lerp(targetDirection, directionToCenter, 0.2f).normalized;
        }

        directionChangeTimer = 0f;
    }

    private Vector3 ApplyConstraints(Vector3 newPosition)
    {
        Vector3 offset = newPosition - startingPosition;
        if (constrainMovementRange && offset.magnitude > movementRange.magnitude)
        {
            offset = offset.normalized * movementRange.magnitude;
            newPosition = startingPosition + offset;
        }
        if (constrainToScreen && mainCamera != null)
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(newPosition);
            float paddingX = screenPaddingInPx / screenBoundsInPx.x;
            float paddingY = screenPaddingInPx / screenBoundsInPx.y;

            viewportPos.x = Mathf.Clamp(viewportPos.x, paddingX, 1f - paddingX);
            viewportPos.y = Mathf.Clamp(viewportPos.y, paddingY, 1f - paddingY);

            newPosition = mainCamera.ViewportToWorldPoint(viewportPos);
            newPosition.z = transform.position.z;
        }
        return newPosition;
    }

    public void SetStartingPosition(Vector3 position)
    {
        startingPosition = position;
    }

    public void ResetToStartingPosition()
    {
        transform.position = startingPosition;
    }

    public void SetMovementEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    // Draw gizmos in the editor to visualize movement range
    void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? startingPosition : transform.position;

        // Draw movement range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, movementRange.magnitude);

        // Draw starting position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, 0.1f);

        // Draw current direction if playing
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, currentDirection);
        }
    }
}