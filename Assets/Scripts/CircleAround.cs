using UnityEngine;
using System.Collections.Generic;

public class CircleAround : MonoBehaviour
{
    public Transform target;      // The object to circle around
    public float radius = 2f;     // Distance from the target
    public float speed = 1f;      // Speed of rotation (radians per second)
    public float fadeDuration = 1f;
    [Header("Circle Drawing")]
    public float circleLineWidth = 0.1f;

    private float angle = 0f;
    private SpriteRenderer spriteRenderer;
    private float fadeTimer = 0f;
    private bool fadingIn = true;
    private bool fadingOut = false;

    // Loop tracking variables
    private int completedLoops = 0;
    private float lastAngle = 0f;
    private bool hasCompletedFirstRotation = false;

    // Circle drawing variables
    private LineRenderer circleLineRenderer;
    private GameObject circleObject;
    private List<Vector3> trailPoints;
    private List<Color> trailColors = new List<Color> {
        ColorPalette.HotPink,
        ColorPalette.ElectricCyan,
        ColorPalette.BrightYellow
    };

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0f;
            spriteRenderer.color = c;
        }
        fadeTimer = 0f;
        fadingIn = true;
        fadingOut = false;
        lastAngle = angle;
        trailPoints = new List<Vector3>();
        CreateCircleDrawingObject();
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        // Fade in
        if (fadingIn)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            SetAlpha(alpha);
            if (alpha >= 1f)
            {
                fadingIn = false;
            }
        }
        // Fade out
        else if (fadingOut)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (fadeTimer / fadeDuration));
            SetAlpha(alpha);
            if (alpha <= 0f)
            {
                Destroy(gameObject);
            }
        }

        // Start fade out if target is null
        if (!fadingOut && target == null)
        {
            fadingOut = true;
            fadeTimer = 0f;
        }

        // Only circle if not fading out and target exists
        if (!fadingOut && target != null)
        {
            angle -= speed * Time.deltaTime;
            CheckForCompletedLoop();
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            transform.position = target.position + new Vector3(x, y, 0f);
            if (completedLoops % 4 < 3)
            {
                AddPointToTrail();
            }
            else
            {
                ClearTrail();
            }
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }

    private void CreateCircleDrawingObject()
    {
        // Create a separate GameObject for the circle line renderer
        circleObject = new GameObject("DrawnCircle");
        circleObject.transform.SetParent(transform);

        // Add and configure LineRenderer
        circleLineRenderer = circleObject.AddComponent<LineRenderer>();
        Material circleMaterial = new Material(Shader.Find("Sprites/Default"));
        circleMaterial.color = trailColors[0];
        circleLineRenderer.material = circleMaterial;
        circleLineRenderer.startWidth = circleLineWidth;
        circleLineRenderer.endWidth = circleLineWidth;
        circleLineRenderer.positionCount = 0; // Start with no points
        circleLineRenderer.useWorldSpace = true;
    }

    private void CheckForCompletedLoop()
    {
        // Normalize angles to 0-2π range for comparison
        float normalizedAngle = angle % (2 * Mathf.PI);
        float normalizedLastAngle = lastAngle % (2 * Mathf.PI);

        // Check if we've crossed the 0/2π boundary (completed a full rotation)
        // We're rotating in the negative direction, so check if we've gone from a small positive angle to a large positive angle
        if (hasCompletedFirstRotation && normalizedLastAngle < normalizedAngle)
        {
            completedLoops++;
            ClearTrail();
            if (completedLoops % 4 == 0)
            {
                circleLineRenderer.material.color = trailColors[0];
                speed = 3.5f;
            }
            else if (completedLoops % 4 == 1)
            {
                circleLineRenderer.material.color = trailColors[1];
                speed = 5f;
            }
            else if (completedLoops % 4 == 2)
            {
                circleLineRenderer.material.color = trailColors[2];
                speed = 6.5f;
            }
            else if (completedLoops % 4 == 3)
            {
                speed = 5f;
            }
        }

        // Mark that we've started rotating (after the first significant movement)
        if (!hasCompletedFirstRotation && Mathf.Abs(angle - 0f) > 0.1f)
        {
            hasCompletedFirstRotation = true;
        }

        lastAngle = angle;
    }

    private void AddPointToTrail()
    {
        if (circleLineRenderer == null) return;
        trailPoints.Add(transform.position);
        circleLineRenderer.positionCount = trailPoints.Count;
        circleLineRenderer.SetPositions(trailPoints.ToArray());
    }

    private void ClearTrail()
    {
        if (circleLineRenderer == null) return;

        // Clear all trail points
        trailPoints.Clear();
        circleLineRenderer.positionCount = 0;
    }

    private void OnDestroy()
    {
        // Clean up the circle object
        if (circleObject != null)
        {
            Destroy(circleObject);
        }
    }
}