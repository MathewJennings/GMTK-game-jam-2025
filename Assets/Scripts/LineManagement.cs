using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LineManagement : MonoBehaviour
{
    [SerializeField]
    float timeToFade;

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    private bool finishedDrawing = false;
    private readonly List<Vector2> drawPositions = new(150);
    private readonly List<float> drawTimes = new(150);

    private void Awake()
    {
        InitializeLine();
    }

    private void InitializeLine()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        drawPositions.Clear();

        drawPositions.Add(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        lineRenderer.SetPosition(0, drawPositions[0]);
        drawTimes.Add(Time.time);

        drawPositions.Add(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        lineRenderer.SetPosition(1, drawPositions[1]);
        drawTimes.Add(Time.time);

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
            }
        }
    }

    private void CheckFadeOldPoints()
    {
        for (int i = 0; i < drawTimes.Count; i++)
        {
            if (Time.time - drawTimes[i] > timeToFade)
            {
                RemovePointFromLine(i);
            }
        }
    }

    private void AddNewPointToLine(Vector2 currentPosition, float currentTime)
    {
        if (drawPositions.Count >= 150)
        {
            drawPositions.RemoveAt(0);
            drawTimes.RemoveAt(0);
        }
        else
        {
            lineRenderer.positionCount++;
        }
        drawPositions.Add(currentPosition);
        drawTimes.Add(currentTime);
        lineRenderer.SetPositions(Vector2ListToVector3List(drawPositions).ToArray());
        edgeCollider.points = drawPositions.ToArray();
    }

    private void RemovePointFromLine(int i)
    {
        drawPositions.RemoveAt(i);
        drawTimes.RemoveAt(i);
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
