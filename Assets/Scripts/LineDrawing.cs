using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LineDrawing : MonoBehaviour
{
    [SerializeField]
    int maxLineLength;

    [SerializeField]
    float timeToFade;

    [SerializeField]
    float minimumLoopArea = 1.0f;

    private LineRenderer lineRenderer;
    private LineGradient lineGradient;
    private EdgeCollider2D edgeCollider;
    private GameObject audioManager;

    private bool finishedDrawing = false;
    private List<Vector2> drawPositions;
    private List<float> drawTimes;
    private List<bool> drawValidForLoops;

    /// Cannot be less than 1 second
    public void SetTimeToFade(float time)
    {
        timeToFade = time < 1 ? 1 : time;
    }

    /// Cannot be less than 150
    public void SetMaxLineLength(int length)
    {
        maxLineLength = length < 150 ? 150 : length;
        InitializeLine();
    }

    /// <summary>
    /// Must set the audio manager after instantiating the line drawing for playing audio feedback when a loop is created.
    /// </summary>
    /// <param name="audioManager">The Audio Manager GameObject.</param>
    public void SetAudioManager(GameObject audioManager)
    {
        this.audioManager = audioManager;
    }

    private void Awake()
    {
        InitializeLine();
    }

    private void InitializeLine()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        lineGradient = GetComponent<LineGradient>();
        lineGradient.SetMaxLineLength(maxLineLength);

        drawPositions = new(maxLineLength);
        drawTimes = new(maxLineLength);
        drawValidForLoops = new(maxLineLength);

        drawPositions.Add(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        lineRenderer.SetPosition(0, drawPositions[0]);
        drawTimes.Add(Time.time);
        drawValidForLoops.Add(true);

        drawPositions.Add(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        lineRenderer.SetPosition(1, drawPositions[1]);
        drawTimes.Add(Time.time);
        drawValidForLoops.Add(true);

        edgeCollider.points = drawPositions.ToArray();
    }



    private void Update()
    {
        CheckFinishedDrawing();
        CheckKeepDrawing();
        CheckFadeOldPoints();
    }

    private void CheckFinishedDrawing()
    {
        if (!finishedDrawing && !Mouse.current.leftButton.IsPressed())
        {
            finishedDrawing = true;
        }
    }

    private void CheckKeepDrawing()
    {
        if (!finishedDrawing && Mouse.current.leftButton.IsPressed())
        {
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if (Vector2.Distance(currentPosition, drawPositions[^1]) > .1f)
            {
                AddNewPointToLine(currentPosition, Time.time);
                lineGradient.UpdateGradient();
            }
        }
    }

    private void AddNewPointToLine(Vector2 currentPosition, float currentTime)
    {
        if (drawPositions.Count >= maxLineLength)
        {
            drawPositions.RemoveAt(0);
            drawTimes.RemoveAt(0);
            drawValidForLoops.RemoveAt(0);
        }
        else
        {
            lineRenderer.positionCount++;
        }
        drawPositions.Add(currentPosition);
        drawTimes.Add(currentTime);
        drawValidForLoops.Add(true);
        lineRenderer.SetPositions(Vector2ListToVector3List(drawPositions).ToArray());
        edgeCollider.points = drawPositions.ToArray();

        if (CreatedLoop())
        {
            Debug.Log("Loop created!");
            AudioSource audioSource = audioManager.GetComponent<AudioSource>();
            audioSource.Play();
        }
    }

    // Check if the very last point in drawPositions is close to any other point.
    // Ignore the 15 latest points to avoid false positives.
    private bool CreatedLoop()
    {
        if (drawPositions.Count < 15) return false;

        Vector2 referencePoint = drawPositions[^1];
        // Start checking at 15 from the end.
        for (int i = drawPositions.Count - 15; i >= 0; i--)
        {
            // Return false if this point was already used in a previous loop.
            if (!drawValidForLoops[i]) return false;

            if (Vector2.Distance(referencePoint, drawPositions[i]) < 0.1f)
            {
                // An intersection was found. Invalidate all points up until now.
                for (int j = 0; j < drawValidForLoops.Count; j++)
                {
                    drawValidForLoops[j] = false;
                }

                // Check if the loop with all the points involved has a large enough area.
                float area = CalculatePolygonArea(
                    drawPositions.GetRange(i, drawPositions.Count - i));
                return area > minimumLoopArea;
            }
        }

        return false;
    }

    // Calculate the area of a polygon using the Shoelace formula.
    private float CalculatePolygonArea(List<Vector2> points)
    {
        float area = 0f;
        int count = points.Count;

        for (int i = 0; i < count; i++)
        {
            Vector2 current = points[i];
            Vector2 next = points[(i + 1) % count]; // Wrap around to the first point.
            area += current.x * next.y - next.x * current.y;
        }

        return Mathf.Abs(area) / 2f;
    }

    private void CheckFadeOldPoints()
    {
        // Iterate backwards to avoid index shifting issues when removing elements
        for (int i = drawTimes.Count - 1; i >= 0; i--)
        {
            if (Time.time - drawTimes[i] > timeToFade)
            {
                RemovePointFromLine(i);
            }
        }
    }

    private void RemovePointFromLine(int i)
    {
        drawPositions.RemoveAt(i);
        drawTimes.RemoveAt(i);
        drawValidForLoops.RemoveAt(i);
        lineRenderer.positionCount = drawPositions.Count;
        lineRenderer.SetPositions(Vector2ListToVector3List(drawPositions).ToArray());
        edgeCollider.points = drawPositions.ToArray();
        if (drawPositions.Count <= 2)
        {
            Destroy(gameObject);
        }
    }

    private List<Vector3> Vector2ListToVector3List(List<Vector2> vector2s)
    {
        List<Vector3> vector3s = new(vector2s.Count);
        for (int i = 0; i < vector2s.Count; i++)
        {
            vector3s.Add(vector2s[i]);
        }
        return vector3s;
    }
}
