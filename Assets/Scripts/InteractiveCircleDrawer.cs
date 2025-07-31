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
        
        // Get starting position
        Vector2 mousePos = mouse.position.ReadValue();
        startPoint = drawingCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        drawnPoints.Add(startPoint);
        
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPoint);
        
        while (isDrawing)
        {
            if (mouse.leftButton.isPressed)
            {
                Vector2 currentMousePos = mouse.position.ReadValue();
                Vector3 worldPos = drawingCamera.ScreenToWorldPoint(new Vector3(currentMousePos.x, currentMousePos.y, 10f));
                
                // Only add point if it's far enough from the last point
                if (drawnPoints.Count == 0 || Vector3.Distance(worldPos, drawnPoints[drawnPoints.Count - 1]) > drawingThreshold)
                {
                    drawnPoints.Add(worldPos);
                    lineRenderer.positionCount = drawnPoints.Count;
                    lineRenderer.SetPosition(drawnPoints.Count - 1, worldPos);
                    
                    // Update visual effects as we draw
                    UpdateLineVisuals();
                    
                    // Check for self-intersection (only if we have enough points)
                    if (drawnPoints.Count > 4 && CheckForSelfIntersection())
                    {
                        Debug.Log("Self-intersection detected! Ending drawing.");
                        isDrawing = false;
                        
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
}
