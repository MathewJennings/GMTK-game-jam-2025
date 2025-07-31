using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawLine : MonoBehaviour
{
    [SerializeField]
    GameObject linePrefab;

    [SerializeField]
    float timeToFade;

    private GameObject currentLine;
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    private List<Vector2> drawPositions = new(150);
    private List<float> drawTimes = new(150);

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CreateLine();
        }
        if (Mouse.current.leftButton.IsPressed())
        {
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if (Vector2.Distance(currentPosition, drawPositions[drawPositions.Count - 1]) > .1f)
            {
                AddNewPointToLine(currentPosition, Time.time);
            }
        }
        for (int i = 0; i < drawTimes.Count; i++)
        {
            if (Time.time - drawTimes[i] > timeToFade)
            {
                RemovePointFromLine(i);
            }
        }
    }

    void CreateLine()
    {
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        drawPositions.Clear();
        drawPositions.Add(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        drawPositions.Add(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        lineRenderer.SetPosition(0, drawPositions[0]);
        lineRenderer.SetPosition(1, drawPositions[1]);
        edgeCollider.points = drawPositions.ToArray();
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
        lineRenderer.SetPositions(vector2ToVector3(drawPositions).ToArray());
        edgeCollider.points = drawPositions.ToArray();
    }

    private void RemovePointFromLine(int i)
    {
        drawPositions.RemoveAt(i);
        drawTimes.RemoveAt(i);
        lineRenderer.SetPositions(vector2ToVector3(drawPositions).ToArray());
        edgeCollider.points = drawPositions.ToArray();
        if (drawPositions.Count == 2)
        {
            GameObject.Destroy(currentLine);
        }
    }

    private List<Vector3> vector2ToVector3(List<Vector2> vector2s)
    {
        List<Vector3> vector3s = new List<Vector3>(drawPositions.Count);
        for (int i = 0; i < vector2s.Count; i++)
        {
            vector3s.Add(new Vector3(vector2s[i].x, vector2s[i].y, 0f));
        }
        return vector3s;
    }
}
