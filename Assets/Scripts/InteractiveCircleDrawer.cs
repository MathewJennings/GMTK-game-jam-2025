using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteractiveCircleDrawer : MonoBehaviour
{
    [Header("Drawing Settings")]
    public LineRenderer lineRenderer;
    public float drawingThreshold = 0.1f; // Minimum distance between points
    public float intersectionTolerance = 0.2f; // How close lines need to be to count as intersection
    public Camera drawingCamera;
    
    [Header("Collision Settings")]
    [Tooltip("Enable collision detection for the drawn circle")]
    public bool enableCollisionDetection = true;
    [Tooltip("Radius of the collision detection around each line segment")]
    public float collisionRadius = 0.1f;
    [Tooltip("Layer mask for objects that can break the circle")]
    public LayerMask breakingObjectsLayer = -1;
    [Tooltip("Effect to spawn when circle is broken (optional)")]
    public GameObject breakEffect;
    [Tooltip("Prefab for the line tip collider (optional - will be created automatically if not provided)")]
    public GameObject lineTipPrefab;
    
    [Header("Visual Settings")]
    public Gradient lineColorGradient; // Gradient for the line color
    [Range(0.01f, 1.0f)]
    public float lineWidth = 0.05f; // Thicker line for more impact (now with smaller default)
    public AnimationCurve lineWidthCurve; // Width variation along the line
    
    [Header("UI Elements")]
    public Text resultText; // For displaying circle analysis results
    public Text centerScoreText; // Text to display in the center of the circle
    public Canvas worldCanvas; // Canvas for world space UI elements
    
    private List<Vector3> drawnPoints = new List<Vector3>();
    private List<GameObject> colliderObjects = new List<GameObject>(); // Store collider GameObjects
    private EdgeCollider2D edgeCollider; // Main collider for the drawn line
    private GameObject lineTip; // Moving tip for collision detection
    private bool isDrawing = false;
    private Vector3 startPoint;
    
    // Input System variables
    private Mouse mouse;
    private Pointer pointer;
    
    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
        }
        
        if (drawingCamera == null)
        {
            drawingCamera = Camera.main;
        }
        
        // Initialize Input System
        mouse = Mouse.current;
        pointer = Pointer.current;
        
        // Initialize EdgeCollider2D for collision detection
        if (enableCollisionDetection)
        {
            edgeCollider = GetComponent<EdgeCollider2D>();
            if (edgeCollider == null)
            {
                edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
                Debug.Log("Created new EdgeCollider2D component");
            }
            else
            {
                Debug.Log("Found existing EdgeCollider2D component");
            }
            
            edgeCollider.enabled = false; // Start disabled, enable when drawing starts
            edgeCollider.isTrigger = true; // IMPORTANT: Must be a trigger for OnTriggerEnter2D to work
            
            // Setup LineTip for collision detection
            SetupLineTip();
            
            Debug.Log($"EdgeCollider2D setup complete - GameObject: {gameObject.name}, Layer: {gameObject.layer}");
            Debug.Log($"Breaking objects layer mask: {breakingObjectsLayer.value}");
        }
        else
        {
            Debug.Log("Collision detection is disabled");
        }
        
        // Initialize gradient if not set
        if (lineColorGradient == null || lineColorGradient.colorKeys.Length == 0)
        {
            lineColorGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.cyan, 0.0f);
            colorKeys[1] = new GradientColorKey(Color.magenta, 0.5f);
            colorKeys[2] = new GradientColorKey(Color.yellow, 1.0f);
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
            alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);
            
            lineColorGradient.SetKeys(colorKeys, alphaKeys);
        }
        
        // Initialize width curve if not set
        if (lineWidthCurve == null || lineWidthCurve.keys.Length == 0)
        {
            lineWidthCurve = new AnimationCurve();
            lineWidthCurve.AddKey(0f, 1f);   // Start at normal width
            lineWidthCurve.AddKey(0.5f, 1f); // Stay at normal width in middle
            lineWidthCurve.AddKey(1f, 1f);   // End at normal width
        }
        
        // Configure line renderer
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        
        // Only apply width curve if it's set, otherwise use flat width
        if (lineWidthCurve != null && lineWidthCurve.keys.Length > 1)
        {
            lineRenderer.widthCurve = lineWidthCurve;
        }
        else
        {
            // Create a flat curve so the width stays consistent
            AnimationCurve flatCurve = new AnimationCurve();
            flatCurve.AddKey(0f, 1f);
            flatCurve.AddKey(1f, 1f);
            lineRenderer.widthCurve = flatCurve;
        }
        
        lineRenderer.colorGradient = lineColorGradient;
        
        // Create and assign material with better visual properties
        Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineMaterial.color = Color.white;
        lineRenderer.material = lineMaterial;
        
        // Enable smooth curves and better quality
        lineRenderer.numCornerVertices = 4;
        lineRenderer.numCapVertices = 4;
        lineRenderer.useWorldSpace = true;
    }
    
    void Update()
    {
        if (mouse != null && mouse.leftButton.wasPressedThisFrame && !isDrawing)
        {
            StartCoroutine(DrawCircleCoroutine());
        }
    }
    
    IEnumerator DrawCircleCoroutine()
    {
        isDrawing = true;
        drawnPoints.Clear();
        
        // Enable collision detection if enabled
        if (enableCollisionDetection && edgeCollider != null)
        {
            // TEMPORARY: Disable collision detection to test if this is causing the issue
            edgeCollider.enabled = true;
            Debug.Log($"EdgeCollider2D enabled for drawing. IsTrigger: {edgeCollider.isTrigger}");
            // Debug.Log("COLLISION DETECTION TEMPORARILY DISABLED FOR DEBUGGING");
        }
        else
        {
            Debug.Log($"EdgeCollider2D NOT enabled - enableCollisionDetection: {enableCollisionDetection}, edgeCollider null: {edgeCollider == null}");
        }
        
        // Get starting position FIRST
        Vector2 mousePos = mouse.position.ReadValue();
        Debug.Log($"Initial mouse screen position: {mousePos}");
        
        // Safety check for camera and mouse position
        if (drawingCamera == null)
        {
            Debug.LogError("Drawing camera is null! Cannot convert screen to world position.");
            yield break;
        }
        
        startPoint = drawingCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        drawnPoints.Add(startPoint);
        
        Debug.Log($"Starting drawing at mouse position: {startPoint}");
        
        // Activate LineTip for collision detection AFTER we have startPoint
        if (enableCollisionDetection && lineTip != null)
        {
            lineTip.SetActive(true);
            
            // Use Rigidbody2D to position the LineTip initially at the MOUSE CLICK position
            Rigidbody2D tipRigidbody = lineTip.GetComponent<Rigidbody2D>();
            if (tipRigidbody != null)
            {
                tipRigidbody.position = new Vector2(startPoint.x, startPoint.y);
                Debug.Log($"LineTip Rigidbody positioned at: {tipRigidbody.position}");
            }
            else
            {
                lineTip.transform.position = startPoint;
                Debug.Log($"LineTip Transform positioned at: {lineTip.transform.position}");
            }
            
            Debug.Log($"LineTip activated at position: {startPoint}");
            Debug.Log($"LineTip layer: {lineTip.layer}");
            Debug.Log($"LineTip active in hierarchy: {lineTip.activeInHierarchy}");
            
            // Check the collider setup
            CircleCollider2D tipCollider = lineTip.GetComponent<CircleCollider2D>();
            if (tipCollider != null)
            {
                Debug.Log($"LineTip collider - IsTrigger: {tipCollider.isTrigger}, Radius: {tipCollider.radius}");
            }
            else
            {
                Debug.LogError("LineTip has no CircleCollider2D!");
            }
        }
        
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPoint);
        
        while (isDrawing)
        {
            if (mouse.leftButton.isPressed)
            {
                Vector2 currentMousePos = mouse.position.ReadValue();
                
                // Safety check for camera
                if (drawingCamera == null)
                {
                    Debug.LogError("Drawing camera became null during drawing loop!");
                    break;
                }
                
                Vector3 worldPos = drawingCamera.ScreenToWorldPoint(new Vector3(currentMousePos.x, currentMousePos.y, 10f));
                
                // Debug coordinate conversion to identify (0,0,0) issue
                Debug.Log($"Drawing loop - Mouse screen pos: {currentMousePos}, World pos: {worldPos}");
                
                // Safety check: don't move LineTip if mouse position is invalid (0,0) or world position is (0,0,0)
                bool isValidMousePosition = currentMousePos.magnitude > 0.1f && (Mathf.Abs(worldPos.x) > 0.01f || Mathf.Abs(worldPos.y) > 0.01f);
                
                // Update LineTip position to follow mouse (only if we have a valid position)
                if (enableCollisionDetection && lineTip != null && lineTip.activeInHierarchy && isValidMousePosition)
                {
                    // Use Rigidbody2D to move the LineTip smoothly instead of teleporting
                    Rigidbody2D tipRigidbody = lineTip.GetComponent<Rigidbody2D>();
                    if (tipRigidbody != null)
                    {
                        // Move to the new position using MovePosition for proper collision detection
                        tipRigidbody.MovePosition(worldPos);
                    }
                    else
                    {
                        // Fallback to transform if no rigidbody
                        lineTip.transform.position = worldPos;
                    }
                    
                    // Debug every few frames to avoid spam but ensure we see it
                    if (drawnPoints.Count % 5 == 0)
                    {
                        Debug.Log($"LineTip moved to: {worldPos}");
                    }
                }
                else if (enableCollisionDetection)
                {
                    if (!isValidMousePosition)
                    {
                        Debug.LogWarning($"LineTip not updated - invalid mouse position. Screen: {currentMousePos}, World: {worldPos}");
                    }
                    else
                    {
                        Debug.LogWarning($"LineTip not updated - lineTip null: {lineTip == null}, activeInHierarchy: {(lineTip != null ? lineTip.activeInHierarchy.ToString() : "N/A")}");
                    }
                }
                
                // Only add point if it's far enough from the last point
                if (drawnPoints.Count == 0 || Vector3.Distance(worldPos, drawnPoints[drawnPoints.Count - 1]) > drawingThreshold)
                {
                    drawnPoints.Add(worldPos);
                    lineRenderer.positionCount = drawnPoints.Count;
                    lineRenderer.SetPosition(drawnPoints.Count - 1, worldPos);
                    
                    // Update collision detection
                    UpdateCollider();
                    
                    // DISABLED: Manual collision checking as it may cause false positives
                    // if (enableCollisionDetection)
                    // {
                    //     CheckManualCollisions();
                    // }
                    
                    // Update visual effects as we draw
                    UpdateLineVisuals();
                    
                    // Check for self-intersection (only if we have enough points)
                    if (drawnPoints.Count > 4 && CheckForSelfIntersection())
                    {
                        Debug.Log("Self-intersection detected! Ending drawing.");
                        isDrawing = false;
                        
                        // Disable LineTip when drawing ends
                        if (enableCollisionDetection && lineTip != null)
                        {
                            lineTip.SetActive(false);
                        }
                        
                        // Disable collider when circle is complete
                        if (enableCollisionDetection && edgeCollider != null)
                        {
                            edgeCollider.enabled = false;
                        }
                        
                        // Close the circle by connecting back to the start point
                        CloseCircle();
                        
                        AnalyzeDrawnShape();
                        break;
                    }
                }
            }
            else if (mouse.leftButton.wasReleasedThisFrame)
            {
                // Mouse released without intersection - could implement timeout or manual completion here
                Debug.Log("Mouse released, ending drawing.");
                isDrawing = false;
                
                // Disable LineTip when drawing ends
                if (enableCollisionDetection && lineTip != null)
                {
                    lineTip.SetActive(false);
                }
                
                // Disable collider when drawing ends
                if (enableCollisionDetection && edgeCollider != null)
                {
                    edgeCollider.enabled = false;
                }
                
                AnalyzeDrawnShape();
                break;
            }
            
            yield return null;
        }
    }
    
    bool CheckForSelfIntersection()
    {
        if (drawnPoints.Count < 4) return false;
        
        Vector3 currentPoint = drawnPoints[drawnPoints.Count - 1];
        Vector3 previousPoint = drawnPoints[drawnPoints.Count - 2];
        
        // Check if current line segment intersects with any previous line segments (excluding adjacent ones)
        for (int i = 0; i < drawnPoints.Count - 3; i++)
        {
            Vector3 lineStart = drawnPoints[i];
            Vector3 lineEnd = drawnPoints[i + 1];
            
            if (LineSegmentsIntersect(previousPoint, currentPoint, lineStart, lineEnd))
            {
                return true;
            }
        }
        
        // Also check if we're close to the starting point (completing the circle)
        if (Vector3.Distance(currentPoint, startPoint) < intersectionTolerance && drawnPoints.Count > 10)
        {
            return true;
        }
        
        return false;
    }
    
    bool LineSegmentsIntersect(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
    {
        // Convert to 2D for intersection calculation (assuming drawing on XY plane)
        Vector2 a1 = new Vector2(p1.x, p1.y);
        Vector2 a2 = new Vector2(q1.x, q1.y);
        Vector2 b1 = new Vector2(p2.x, p2.y);
        Vector2 b2 = new Vector2(q2.x, q2.y);
        
        Vector2 d1 = a2 - a1;
        Vector2 d2 = b2 - b1;
        
        float denominator = d1.x * d2.y - d1.y * d2.x;
        
        if (Mathf.Abs(denominator) < 0.0001f) return false; // Lines are parallel
        
        float t = ((b1.x - a1.x) * d2.y - (b1.y - a1.y) * d2.x) / denominator;
        float u = ((b1.x - a1.x) * d1.y - (b1.y - a1.y) * d1.x) / denominator;
        
        return t >= 0 && t <= 1 && u >= 0 && u <= 1;
    }
    
    void AnalyzeDrawnShape()
    {
        Debug.Log($"AnalyzeDrawnShape called with {drawnPoints.Count} points");
        
        if (drawnPoints.Count < 3)
        {
            Debug.Log("Not enough points to analyze");
            if (resultText != null)
                resultText.text = "Not enough points to analyze";
            return;
        }
        
        // Calculate center of mass
        Vector3 center = Vector3.zero;
        foreach (Vector3 point in drawnPoints)
        {
            center += point;
        }
        center /= drawnPoints.Count;
        Debug.Log($"Center calculated: {center}");
        
        // Calculate average radius and area
        float totalRadius = 0f;
        float minRadius = float.MaxValue;
        float maxRadius = float.MinValue;
        
        foreach (Vector3 point in drawnPoints)
        {
            float distance = Vector3.Distance(point, center);
            totalRadius += distance;
            minRadius = Mathf.Min(minRadius, distance);
            maxRadius = Mathf.Max(maxRadius, distance);
        }
        
        float averageRadius = totalRadius / drawnPoints.Count;
        Debug.Log($"Radius - Min: {minRadius:F2}, Max: {maxRadius:F2}, Average: {averageRadius:F2}");
        
        // Calculate how circular the shape is (based on radius variation)
        float radiusVariation = maxRadius - minRadius;
        float circularityPercentage = Mathf.Max(0f, 100f - (radiusVariation / averageRadius * 100f));
        
        // Calculate area using shoelace formula
        float area = CalculatePolygonArea(drawnPoints);
        
        // Calculate what the area would be if it were a perfect circle
        float perfectCircleArea = Mathf.PI * averageRadius * averageRadius;
        
        Debug.Log($"Area: {area:F2}, Perfect: {perfectCircleArea:F2}, Circularity: {circularityPercentage:F1}%");
        
        // Calculate fun score (combination of accuracy and size)
        float sizeScore = Mathf.Min(averageRadius * 20f, 100f); // More generous size bonus, capped at 100
        float accuracyMultiplier = circularityPercentage / 100f; // Convert percentage to multiplier (0.0 to 1.0)
        float totalScore = sizeScore * accuracyMultiplier; // Multiply size score by accuracy percentage
        
        // Display center score
        DisplayCenterScore(center, totalScore, circularityPercentage);
        
        // Display results
        string results = "Circle Analysis:\n";
        results += "SCORE: " + totalScore.ToString("F0") + " points\n";
        results += "Circularity: " + circularityPercentage.ToString("F1") + "%\n";
        results += "Area: " + area.ToString("F2") + " units²\n";
        results += "Perfect Circle Area: " + perfectCircleArea.ToString("F2") + " units²\n";
        results += "Average Radius: " + averageRadius.ToString("F2") + "\n";
        results += "Radius Variation: " + radiusVariation.ToString("F2") + "\n";
        results += "Points drawn: " + drawnPoints.Count.ToString();
        
        Debug.Log($"Final results string: {results}");
        
        if (resultText != null)
        {
            resultText.text = results;
            
            // Force the UI to update and adjust settings for better display
            RectTransform rectTransform = resultText.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Ensure the RectTransform has enough height
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, Mathf.Max(200f, rectTransform.sizeDelta.y));
            }
            
            // Ensure text settings are optimal for multi-line display
            resultText.horizontalOverflow = HorizontalWrapMode.Wrap;
            resultText.verticalOverflow = VerticalWrapMode.Overflow;
            
            Debug.Log("Text updated successfully");
            Debug.Log($"Text component text is now: '{resultText.text}'");
            Debug.Log($"Text component enabled: {resultText.enabled}");
            Debug.Log($"Text component gameObject active: {resultText.gameObject.activeInHierarchy}");
            Debug.Log($"RectTransform size: {rectTransform?.sizeDelta}");
        }
        else
        {
            Debug.LogWarning("resultText is null!");
        }
        
        Debug.Log(results);
    }
    
    float CalculatePolygonArea(List<Vector3> points)
    {
        if (points.Count < 3) return 0f;
        
        float area = 0f;
        for (int i = 0; i < points.Count; i++)
        {
            int next = (i + 1) % points.Count;
            area += points[i].x * points[next].y - points[next].x * points[i].y;
        }
        return Mathf.Abs(area) / 2f;
    }
    
    // Method to reset and allow drawing a new circle
    public void ResetDrawing()
    {
        isDrawing = false;
        drawnPoints.Clear();
        lineRenderer.positionCount = 0;
        
        // Disable and clear collider
        if (edgeCollider != null)
        {
            edgeCollider.enabled = false;
            edgeCollider.points = new Vector2[0];
        }
        
        // Clear any collider objects
        foreach (GameObject colliderObj in colliderObjects)
        {
            if (colliderObj != null)
            {
                Destroy(colliderObj);
            }
        }
        colliderObjects.Clear();
        
        // Hide center score text
        if (centerScoreText != null)
        {
            centerScoreText.gameObject.SetActive(false);
        }
        
        if (resultText != null)
            resultText.text = "Click and drag to draw a circle";
    }

    void CloseCircle()
    {
        if (drawnPoints.Count > 2)
        {
            // Check if we're close to the starting point (completing a loop)
            Vector3 currentPoint = drawnPoints[drawnPoints.Count - 1];
            
            if (Vector3.Distance(currentPoint, startPoint) < intersectionTolerance * 3)
            {
                // Close to start point - connect directly to start
                drawnPoints.Add(startPoint);
                lineRenderer.positionCount = drawnPoints.Count;
                lineRenderer.SetPosition(drawnPoints.Count - 1, startPoint);
                Debug.Log("Circle closed by connecting to start point");
            }
            else
            {
                // Find the intersection point and close there
                Vector3 intersectionPoint = FindIntersectionPoint();
                if (intersectionPoint != Vector3.zero)
                {
                    drawnPoints.Add(intersectionPoint);
                    lineRenderer.positionCount = drawnPoints.Count;
                    lineRenderer.SetPosition(drawnPoints.Count - 1, intersectionPoint);
                    Debug.Log("Circle closed at intersection point");
                }
                else
                {
                    // Fallback: connect to start point
                    drawnPoints.Add(startPoint);
                    lineRenderer.positionCount = drawnPoints.Count;
                    lineRenderer.SetPosition(drawnPoints.Count - 1, startPoint);
                    Debug.Log("Circle closed by connecting to start point (fallback)");
                }
            }
        }
    }
    
    Vector3 FindIntersectionPoint()
    {
        if (drawnPoints.Count < 4) return Vector3.zero;
        
        Vector3 currentPoint = drawnPoints[drawnPoints.Count - 1];
        Vector3 previousPoint = drawnPoints[drawnPoints.Count - 2];
        
        // Check for intersection with previous line segments
        for (int i = 0; i < drawnPoints.Count - 3; i++)
        {
            Vector3 lineStart = drawnPoints[i];
            Vector3 lineEnd = drawnPoints[i + 1];
            
            Vector3 intersection = GetLineIntersectionPoint(previousPoint, currentPoint, lineStart, lineEnd);
            if (intersection != Vector3.zero)
            {
                return intersection;
            }
        }
        
        return Vector3.zero;
    }
    
    Vector3 GetLineIntersectionPoint(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
    {
        // Convert to 2D for intersection calculation
        Vector2 a1 = new Vector2(p1.x, p1.y);
        Vector2 a2 = new Vector2(q1.x, q1.y);
        Vector2 b1 = new Vector2(p2.x, p2.y);
        Vector2 b2 = new Vector2(q2.x, q2.y);
        
        Vector2 d1 = a2 - a1;
        Vector2 d2 = b2 - b1;
        
        float denominator = d1.x * d2.y - d1.y * d2.x;
        
        if (Mathf.Abs(denominator) < 0.0001f) return Vector3.zero; // Lines are parallel
        
        float t = ((b1.x - a1.x) * d2.y - (b1.y - a1.y) * d2.x) / denominator;
        float u = ((b1.x - a1.x) * d1.y - (b1.y - a1.y) * d1.x) / denominator;
        
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            // Calculate intersection point
            Vector2 intersectionPoint = a1 + t * d1;
            return new Vector3(intersectionPoint.x, intersectionPoint.y, p1.z);
        }
        
        return Vector3.zero;
    }

    void DisplayCenterScore(Vector3 center, float totalScore, float circularityPercentage)
    {
        if (centerScoreText != null)
        {
            // Position the score text at the center of the circle
            Vector3 screenCenter = drawingCamera.WorldToScreenPoint(center);
            
            if (centerScoreText.transform is RectTransform rectTransform)
            {
                rectTransform.position = screenCenter;
            }
            
            // Create engaging score display
            string scoreDisplay = "";
            
            // Add some flair based on score (more generous thresholds)
            if (totalScore >= 70f)
            {
                scoreDisplay = "*** PERFECT! ***\n";
                centerScoreText.color = Color.yellow;
            }
            else if (totalScore >= 55f)
            {
                scoreDisplay = "** EXCELLENT! **\n";
                centerScoreText.color = Color.green;
            }
            else if (totalScore >= 40f)
            {
                scoreDisplay = "* GREAT! *\n";
                centerScoreText.color = Color.cyan;
            }
            else if (totalScore >= 25f)
            {
                scoreDisplay = "+ GOOD! +\n";
                centerScoreText.color = Color.blue;
            }
            else
            {
                scoreDisplay = "- TRY AGAIN! -\n";
                centerScoreText.color = Color.red;
            }
            
            scoreDisplay += totalScore.ToString("F0") + " POINTS\n";
            scoreDisplay += circularityPercentage.ToString("F1") + "% CIRCULAR";
            
            centerScoreText.text = scoreDisplay;
            centerScoreText.gameObject.SetActive(true);
            
            // Add some animation effect
            StartCoroutine(AnimateScoreText());
        }
    }
    
    IEnumerator AnimateScoreText()
    {
        if (centerScoreText == null) yield break;
        
        // Scale animation
        Vector3 originalScale = centerScoreText.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        
        float animTime = 0f;
        float duration = 0.5f;
        
        // Scale up
        while (animTime < duration)
        {
            animTime += Time.deltaTime;
            float t = animTime / duration;
            centerScoreText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        // Scale back down
        animTime = 0f;
        while (animTime < duration)
        {
            animTime += Time.deltaTime;
            float t = animTime / duration;
            centerScoreText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        centerScoreText.transform.localScale = originalScale;
    }
    
    void UpdateLineVisuals()
    {
        if (lineRenderer != null && drawnPoints.Count > 1)
        {
            // Update the width to current setting
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            
            // Update the gradient colors as the line is drawn
            Gradient currentGradient = new Gradient();
            
            // Create a rainbow effect that progresses as you draw
            float progress = (float)drawnPoints.Count / 100f; // Normalize based on expected max points
            
            GradientColorKey[] colorKeys = new GradientColorKey[4];
            colorKeys[0] = new GradientColorKey(Color.HSVToRGB((progress * 0.1f) % 1f, 0.8f, 1f), 0.0f);
            colorKeys[1] = new GradientColorKey(Color.HSVToRGB((progress * 0.1f + 0.2f) % 1f, 0.8f, 1f), 0.33f);
            colorKeys[2] = new GradientColorKey(Color.HSVToRGB((progress * 0.1f + 0.4f) % 1f, 0.8f, 1f), 0.66f);
            colorKeys[3] = new GradientColorKey(Color.HSVToRGB((progress * 0.1f + 0.6f) % 1f, 0.8f, 1f), 1.0f);
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
            alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);
            
            currentGradient.SetKeys(colorKeys, alphaKeys);
            lineRenderer.colorGradient = currentGradient;
        }
    }
    
    // This allows the line width to update in real-time when you change it in the Inspector
    void OnValidate()
    {
        if (lineRenderer != null && Application.isPlaying)
        {
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }
    }

    // Public method to update line width - useful for UI controls
    public void UpdateLineWidth(float newWidth)
    {
        lineWidth = newWidth;
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }
    }
    
    // Collision Detection Methods
    void UpdateCollider()
    {
        if (enableCollisionDetection && edgeCollider != null && drawnPoints.Count >= 2)
        {
            // Convert Vector3 list to Vector2 array for EdgeCollider2D
            Vector2[] colliderPoints = new Vector2[drawnPoints.Count];
            for (int i = 0; i < drawnPoints.Count; i++)
            {
                colliderPoints[i] = new Vector2(drawnPoints[i].x, drawnPoints[i].y);
            }
            edgeCollider.points = colliderPoints;
            
            if (drawnPoints.Count % 10 == 0) // Only log every 10 points to avoid spam
            {
                Debug.Log($"Updated EdgeCollider2D with {colliderPoints.Length} points. Enabled: {edgeCollider.enabled}, IsTrigger: {edgeCollider.isTrigger}");
                Debug.Log($"First point: {colliderPoints[0]}, Last point: {colliderPoints[colliderPoints.Length - 1]}");
            }
        }
        else if (enableCollisionDetection)
        {
            Debug.Log($"UpdateCollider failed - edgeCollider null: {edgeCollider == null}, points count: {drawnPoints.Count}");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"=== TRIGGER DETECTED ===");
        Debug.Log($"Object: {other.gameObject.name}");
        Debug.Log($"Layer: {other.gameObject.layer}");
        Debug.Log($"Tag: {other.gameObject.tag}");
        Debug.Log($"Position: {other.transform.position}");
        Debug.Log($"Collider type: {other.GetType().Name}");
        Debug.Log($"Has CircleBreaker: {other.GetComponent<CircleBreaker>() != null}");
        
        // IMPORTANT: Never break the circle due to LineTip collisions
        if (other.gameObject.name == "LineTip" || other.gameObject.layer == 8)
        {
            Debug.Log($"TRIGGER IGNORED - LineTip collision detected: {other.gameObject.name}");
            return;
        }
        
        if (enableCollisionDetection && isDrawing && IsBreakingObject(other))
        {
            Debug.Log($"BREAKING CIRCLE due to collision with {other.gameObject.name}");
            BreakCircle(other.transform.position);
        }
        else
        {
            Debug.Log($"Collision IGNORED - enableCollisionDetection: {enableCollisionDetection}, isDrawing: {isDrawing}, IsBreakingObject: {IsBreakingObject(other)}");
        }
    }
    
    bool IsBreakingObject(Collider2D other)
    {
        // IMPORTANT: Never consider LineTip objects as breaking objects
        if (other.gameObject.name == "LineTip" || other.gameObject.layer == 8)
        {
            Debug.Log($"IsBreakingObject: {other.gameObject.name} is LineTip - NOT a breaking object");
            return false;
        }
        
        // Check if the object is on a layer that can break the circle
        bool canBreak = ((1 << other.gameObject.layer) & breakingObjectsLayer) != 0;
        Debug.Log($"Checking if {other.gameObject.name} (layer {other.gameObject.layer}) can break circle. LayerMask: {breakingObjectsLayer.value}, Result: {canBreak}");
        return canBreak;
    }
    
    void BreakCircle(Vector3 breakPosition)
    {
        Debug.Log("=== CIRCLE BROKEN ===");
        Debug.Log($"Break position: {breakPosition}");
        Debug.Log($"Current drawing points: {drawnPoints.Count}");
        Debug.Log($"Is drawing: {isDrawing}");
        
        // Print stack trace to see what called this
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        Debug.Log($"Called from: {stackTrace.ToString()}");
        
        // Spawn break effect if available
        if (breakEffect != null)
        {
            Instantiate(breakEffect, breakPosition, Quaternion.identity);
        }
        
        // Stop drawing and reset
        isDrawing = false;
        
        // Disable collider
        if (edgeCollider != null)
        {
            edgeCollider.enabled = false;
        }
        
        // Clear the drawing
        StartCoroutine(FadeOutBrokenCircle());
    }
    
    IEnumerator FadeOutBrokenCircle()
    {
        // Quick fade out effect
        Color originalColor = lineRenderer.material.color;
        float fadeTime = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            lineRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        // Reset everything
        ResetDrawing();
        lineRenderer.material.color = originalColor;
        
        // Show message to user
        if (resultText != null)
        {
            resultText.text = "Circle broken! Try again.";
        }
    }

    void CheckManualCollisions()
    {
        if (drawnPoints.Count < 2) return;
        
        // Get the most recent line segment
        Vector3 startPoint = drawnPoints[drawnPoints.Count - 2];
        Vector3 endPoint = drawnPoints[drawnPoints.Count - 1];
        
        // Find all objects on the breaking layer
        Collider2D[] breakingObjects = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
        
        foreach (Collider2D collider in breakingObjects)
        {
            if (IsBreakingObject(collider))
            {
                // Check if the line segment intersects with the collider
                if (DoesLineIntersectCollider(startPoint, endPoint, collider))
                {
                    Debug.Log($"Manual collision detected with {collider.gameObject.name}!");
                    BreakCircle(collider.transform.position);
                    return;
                }
            }
        }
    }
    
    bool DoesLineIntersectCollider(Vector3 lineStart, Vector3 lineEnd, Collider2D collider)
    {
        // Simple approach: check if either endpoint is inside the collider
        // or if the collider bounds intersect the line segment
        
        if (collider.OverlapPoint(lineStart) || collider.OverlapPoint(lineEnd))
        {
            return true;
        }
        
        // Check if line intersects collider bounds
        Bounds bounds = collider.bounds;
        Vector3 center = bounds.center;
        float distance = Vector3.Distance(center, lineStart);
        float lineLength = Vector3.Distance(lineStart, lineEnd);
        
        // Simple distance check - if collider center is close to the line segment
        return distance < (bounds.size.magnitude * 0.5f + collisionRadius);
    }

    void SetupLineTip()
    {
        Debug.Log("=== SETTING UP LINETIP ===");
        
        // Create LineTip GameObject
        if (lineTipPrefab != null)
        {
            lineTip = Instantiate(lineTipPrefab, transform);
            Debug.Log("LineTip created from prefab");
        }
        else
        {
            // Create a simple LineTip GameObject
            lineTip = new GameObject("LineTip");
            lineTip.transform.SetParent(transform);
            Debug.Log("LineTip created procedurally");
            
            // Add a simple CircleCollider2D for collision detection
            CircleCollider2D tipCollider = lineTip.AddComponent<CircleCollider2D>();
            tipCollider.isTrigger = true;
            tipCollider.radius = collisionRadius;
            Debug.Log($"LineTip collider added - IsTrigger: {tipCollider.isTrigger}, Radius: {tipCollider.radius}");
            
            // Add Rigidbody2D for proper physics movement and collision detection
            Rigidbody2D tipRigidbody = lineTip.AddComponent<Rigidbody2D>();
            tipRigidbody.gravityScale = 0f; // No gravity
            tipRigidbody.freezeRotation = true; // Don't rotate
            tipRigidbody.bodyType = RigidbodyType2D.Kinematic; // Kinematic so we can control movement
            Debug.Log("LineTip Rigidbody2D added - Kinematic, no gravity");
            
            // Add a bright green visual indicator so we can see the LineTip
            SpriteRenderer tipRenderer = lineTip.AddComponent<SpriteRenderer>();
            
            // Create a simple circle sprite programmatically
            Texture2D circleTexture = new Texture2D(32, 32);
            Vector2 center = new Vector2(16, 16);
            float radius = 14f;
            
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance <= radius)
                    {
                        circleTexture.SetPixel(x, y, Color.green);
                    }
                    else
                    {
                        circleTexture.SetPixel(x, y, Color.clear);
                    }
                }
            }
            circleTexture.Apply();
            
            // Create sprite from texture
            Sprite circleSprite = Sprite.Create(circleTexture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 100);
            tipRenderer.sprite = circleSprite;
            tipRenderer.color = Color.green;
            tipRenderer.sortingOrder = 10; // Render on top of other elements
            
            Debug.Log("LineTip bright green visual indicator added");
            
            // The collider will handle collision detection via OnTriggerEnter2D
            // Add a collision handler component  
            var collisionHandler = lineTip.AddComponent<LineTipCollisionHandler>();
            collisionHandler.parentDrawer = this;
            Debug.Log("LineTipCollisionHandler component added");
            
            // Put LineTip on a different layer to avoid self-collision issues
            // Try to use layer 8 (which is often unused) or default layer
            lineTip.layer = 8; // Change from default layer to avoid collision with parent
            Debug.Log($"LineTip layer set to: {lineTip.layer}");
            
            // Make sure layer 8 is excluded from breaking objects LayerMask
            if (((1 << 8) & breakingObjectsLayer) != 0)
            {
                Debug.LogWarning("Layer 8 (LineTip layer) was included in breakingObjectsLayer! Removing it to prevent self-collision.");
                breakingObjectsLayer &= ~(1 << 8); // Remove layer 8 from the mask
                Debug.Log($"Updated breaking objects layer mask: {breakingObjectsLayer.value}");
            }
        }
        
        // Initially deactivate the LineTip
        lineTip.SetActive(false);
        
        Debug.Log($"LineTip GameObject created - Layer: {lineTip.layer}");
        Debug.Log($"Breaking objects layer mask: {breakingObjectsLayer.value}");
        Debug.Log("LineTip setup complete");
    }
    
    /// <summary>
    /// Called by LineTip when it collides with a breaking object
    /// </summary>
    public void OnLineTipCollision(Vector3 collisionPosition)
    {
        if (isDrawing)
        {
            Debug.Log("Circle broken by LineTip collision!");
            BreakCircle(collisionPosition);
        }
    }
}

/// <summary>
/// Simple collision handler for the LineTip GameObject
/// </summary>
public class LineTipCollisionHandler : MonoBehaviour
{
    public InteractiveCircleDrawer parentDrawer;
    
    void Start()
    {
        Debug.Log($"LineTipCollisionHandler started on {gameObject.name}");
        Debug.Log($"Parent drawer assigned: {parentDrawer != null}");
        
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            Debug.Log($"LineTipCollisionHandler collider setup - IsTrigger: {collider.isTrigger}, Radius: {collider.radius}");
        }
        
        // Log layer and physics settings
        Debug.Log($"LineTip GameObject layer: {gameObject.layer}");
        Debug.Log($"LineTip position: {transform.position}");
    }
    
    void Update()
    {
        // Continuously check for nearby objects to debug collision issues
        if (Time.frameCount % 60 == 0) // Every 60 frames (1 second at 60fps)
        {
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 2.0f);
            Debug.Log($"LineTip nearby objects check - Position: {transform.position}, Found {nearbyColliders.Length} colliders within range");
            
            foreach (Collider2D nearby in nearbyColliders)
            {
                if (nearby.gameObject != gameObject) // Don't include self
                {
                    CircleBreaker breaker = nearby.GetComponent<CircleBreaker>();
                    float distance = Vector3.Distance(transform.position, nearby.transform.position);
                    Debug.Log($"  - {nearby.gameObject.name} (Layer: {nearby.gameObject.layer}, HasCircleBreaker: {breaker != null}, Distance: {distance:F2})");
                    
                    // If there's a CircleBreaker very close, warn about it
                    if (breaker != null && distance < 0.5f)
                    {
                        Debug.LogWarning($"LineTip is very close to CircleBreaker {nearby.gameObject.name} (distance: {distance:F2}) - might cause immediate collision!");
                    }
                }
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"=== LineTip TRIGGER DETECTED ===");
        Debug.Log($"LineTip collision detected with: {other.gameObject.name}");
        Debug.Log($"Other object layer: {other.gameObject.layer}");
        Debug.Log($"Other object tag: {other.gameObject.tag}");
        Debug.Log($"Other object position: {other.transform.position}");
        Debug.Log($"LineTip position: {transform.position}");
        
        // IMPORTANT: Ignore collisions with self and parent drawer
        if (other.gameObject == gameObject)
        {
            Debug.Log("LineTip collision IGNORED - collision with self");
            return;
        }
        
        if (parentDrawer != null && other.gameObject == parentDrawer.gameObject)
        {
            Debug.Log("LineTip collision IGNORED - collision with parent drawer (EdgeCollider2D)");
            return;
        }
        
        // Also ignore if the other object is the LineTip itself (shouldn't happen but just in case)
        if (other.gameObject.name == "LineTip")
        {
            Debug.Log("LineTip collision IGNORED - collision with another LineTip");
            return;
        }
        
        // Check if it's a circle breaker
        CircleBreaker breaker = other.GetComponent<CircleBreaker>();
        Debug.Log($"Has CircleBreaker component: {breaker != null}");
        
        if (breaker != null && parentDrawer != null)
        {
            Debug.Log($"LineTip hit CircleBreaker: {other.gameObject.name} - calling parentDrawer.OnLineTipCollision");
            parentDrawer.OnLineTipCollision(transform.position);
        }
        else
        {
            Debug.Log($"LineTip collision ignored - breaker null: {breaker == null}, parentDrawer null: {parentDrawer == null}");
        }
    }
    
    // Also try OnTriggerStay2D in case Enter isn't working
    void OnTriggerStay2D(Collider2D other)
    {
        // IMPORTANT: Ignore collisions with self and parent drawer
        if (other.gameObject == gameObject || 
            (parentDrawer != null && other.gameObject == parentDrawer.gameObject) ||
            other.gameObject.name == "LineTip")
        {
            return; // Ignore self-collisions
        }
        
        CircleBreaker breaker = other.GetComponent<CircleBreaker>();
        if (breaker != null && parentDrawer != null)
        {
            Debug.Log($"LineTip STAYING in trigger with CircleBreaker: {other.gameObject.name}");
            // Only call once to avoid spam
            if (Time.frameCount % 30 == 0)
            {
                parentDrawer.OnLineTipCollision(transform.position);
            }
        }
    }
    
    // Also add OnCollisionEnter2D as a backup
    void OnCollisionEnter2D(Collision2D collision)
    {
        // IMPORTANT: Ignore collisions with self and parent drawer
        if (collision.gameObject == gameObject || 
            (parentDrawer != null && collision.gameObject == parentDrawer.gameObject) ||
            collision.gameObject.name == "LineTip")
        {
            Debug.Log($"LineTip solid collision IGNORED - collision with {collision.gameObject.name}");
            return; // Ignore self-collisions
        }
        
        Debug.Log($"LineTip COLLISION (not trigger) detected with: {collision.gameObject.name}");
        CircleBreaker breaker = collision.gameObject.GetComponent<CircleBreaker>();
        if (breaker != null && parentDrawer != null)
        {
            Debug.Log($"LineTip collision (solid) with CircleBreaker: {collision.gameObject.name}");
            parentDrawer.OnLineTipCollision(transform.position);
        }
    }
}
