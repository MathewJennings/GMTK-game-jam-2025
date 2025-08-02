using System;
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
    float timeToFadeWhenFinishedDrawing;

    private LineRenderer lineRenderer;
    private LineGradient lineGradient;
    private EdgeCollider2D edgeCollider;
    private LoopCounter loopCounter;
    private LoopDetector loopDetector;

    private bool finishedDrawing = false;
    private int numPointsToRemoveWhenFinishedDrawing = 0;
    private float timeElapsedSinceFinishedDrawing = 0f;
    private List<Vector2> drawPositions;
    private List<float> drawTimes;
    private List<bool> drawValidForLoops;

    private Action<int> notifyOnLineDrawingEnded;
    public void SetNotifyOnLineDrawingEnded(Action<int> notifyOnLineDrawingEnded)
    {
        this.notifyOnLineDrawingEnded = notifyOnLineDrawingEnded;
    }

    /// Cannot be less than 1 second
    public void SetTimeToFade(float time)
    {
        timeToFade = time < 1 ? 1 : time;
    }

    public void SetTimeToFadeWhenFinishedDrawing(float time)
    {
        timeToFadeWhenFinishedDrawing = time > 1 ? 1 : time;
    }

    /// Cannot be less than 150
    public void SetMaxLineLength(int length)
    {
        maxLineLength = length < 150 ? 150 : length;
        InitializeLine();
    }

    private void Awake()
    {
        InitializeLine();
    }

    private void InitializeLine()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        loopCounter = GetComponent<LoopCounter>();
        loopDetector = GetComponent<LoopDetector>();
        lineGradient = GetComponent<LineGradient>();

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
            FinishDrawing();
        }
        if (finishedDrawing && drawPositions.Count >= 2)
        {
            EraseFinishedLine();
        }
    }

    private void FinishDrawing()
    {
        finishedDrawing = true;
        numPointsToRemoveWhenFinishedDrawing = drawPositions.Count;
        timeElapsedSinceFinishedDrawing = 0f;
        notifyOnLineDrawingEnded(drawPositions.Count);
    }

    private void EraseFinishedLine()
    {
        timeElapsedSinceFinishedDrawing += Time.deltaTime;
        float fadeProgress = timeElapsedSinceFinishedDrawing / timeToFadeWhenFinishedDrawing;
        fadeProgress = Mathf.Clamp01(fadeProgress);
        // Apply an ease-out curve (starts fast, slows down)
        float curvedProgress = 1f - Mathf.Pow(1f - fadeProgress, 3f);
        int targetPointsRemoved = Mathf.RoundToInt(curvedProgress * numPointsToRemoveWhenFinishedDrawing);
        int currentPointsRemoved = numPointsToRemoveWhenFinishedDrawing - drawPositions.Count;
        int pointsToRemove = targetPointsRemoved - currentPointsRemoved;
        if (pointsToRemove > 0)
        {
            for (int i = pointsToRemove; i > 0 && drawPositions.Count >= 2; i--)
            {
                RemovePointFromLine(i - 1);
            }
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
                loopCounter.UpdateLineTipPosition(currentPosition);
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
        loopDetector.CheckCreatedLoop(drawPositions, drawValidForLoops);
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
            DestroyLine();
        }
    }

    public void DestroyLine()
    {
        Destroy(gameObject);
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
