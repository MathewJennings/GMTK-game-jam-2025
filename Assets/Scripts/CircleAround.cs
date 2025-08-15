using UnityEngine;
using System.Collections.Generic;

public class CircleAround : MonoBehaviour
{
    public Transform target;      // The object to circle around
    public float radius = 2f;     // Distance from the target
    public float speed = 1f;      // Speed of rotation (radians per second)
    public Sprite mouseSprite;
    public Sprite leftClickMouseSprite;
    [Header("Circle Drawing")]
    public float circleLineWidth = 0.1f;

    private float angle = 0f;
    private SpriteRenderer spriteRenderer;

    // Loop tracking variables
    private int completedLoops = 0;
    private float lastAngle = 0f;
    private bool hasCompletedFirstRotation = false;

    // Circle drawing variables
    private readonly List<LineRenderer> circleLineRenderers = new();
    private readonly Vector3 drawOffset = new(-0.25f, .15f);
    private GameObject circleObject;
    private readonly List<Vector3> trailPoints = new();
    private readonly List<Color> trailColors = new()
    {
        ColorPalette.HotPink,
        ColorPalette.ElectricCyan,
        ColorPalette.BrightYellow
    };

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastAngle = angle;
        CreateCircleDrawingObject();
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        float[] xOffsets = { -0.04f, 0.04f, -0.02f, 0.02f };
        float[] yOffsets = { 0.09f, 0f, -0.09f, -0.18f };
        int currentIndex = completedLoops % 4;
        int nextIndex = (completedLoops + 1) % 4;
        float normalizedAngle = Mathf.Abs(angle % (2 * Mathf.PI));
        float loopProgress = normalizedAngle / (2 * Mathf.PI);
        float xOffset = Mathf.Lerp(xOffsets[currentIndex], xOffsets[nextIndex], loopProgress);
        float yOffset = Mathf.Lerp(yOffsets[currentIndex], yOffsets[nextIndex], loopProgress);

        if (target != null)
        {
            angle -= speed * Time.deltaTime;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            transform.position = target.position + new Vector3(x + xOffset, y + yOffset, 0f) - drawOffset;
            if (completedLoops % 4 < 3)
            {
                AddPointToTrail();
            }
            else
            {
                ClearTrail();
            }
            CheckForCompletedLoop();
        }
    }

    private void CreateCircleDrawingObject()
    {
        for (int i = 0; i < 3; i++)
        {
            circleObject = new GameObject($"DrawnCircle {i}");
            circleObject.transform.SetParent(transform);
            circleObject.transform.Translate(-0.05f, 0.05f, 0f);

            LineRenderer circleLineRenderer = circleObject.AddComponent<LineRenderer>();
            circleLineRenderer.material = new Material(Shader.Find("Sprites/Default"))
            {
                color = trailColors[i]
            };
            circleLineRenderer.sortingOrder = i;
            circleLineRenderer.startWidth = circleLineWidth;
            circleLineRenderer.endWidth = circleLineWidth;
            circleLineRenderer.positionCount = 0; // Start with no points
            circleLineRenderer.useWorldSpace = true;
            circleLineRenderers.Add(circleLineRenderer);
        }
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
            if (trailPoints.Count > 0)
            {
                Vector3 lastPoint = trailPoints[^1];
                trailPoints.Clear();
                trailPoints.Add(lastPoint);
            }
            if (completedLoops % 4 == 3)
            {
                speed = 3.5f;
            }
            else
            {
                speed = 5f;
                spriteRenderer.sprite = leftClickMouseSprite;
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
        if (circleLineRenderers[completedLoops % 4] == null) return;
        trailPoints.Add(transform.position + drawOffset);
        circleLineRenderers[completedLoops % 4].positionCount = trailPoints.Count;
        circleLineRenderers[completedLoops % 4].SetPositions(trailPoints.ToArray());
    }

    private void ClearTrail()
    {
        float normalizedAngle = Mathf.Abs(angle % (2 * Mathf.PI));
        // After the 1st 1/4th circle, take 1/16th circle to complete a fade
        float loopProgress = normalizedAngle / (Mathf.PI/8) - Mathf.PI/2;
        float alpha = Mathf.Lerp(1, 0, loopProgress);
        for (int i = 0; i < 3; i++)
        {
            if (circleLineRenderers[i] == null) continue;
            Color color = circleLineRenderers[i].material.color;
            color.a = alpha;
            circleLineRenderers[i].material.color = color;
        }
        if (loopProgress >= 1) {
            for (int i = 0; i < 3; i++)
            {
                circleLineRenderers[i].positionCount = 0;
            }
            trailPoints.Clear();
            spriteRenderer.sprite = mouseSprite;
        }

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