using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawLine : MonoBehaviour
{
    [SerializeField]
    GameObject linePrefab;

    private GameObject currentLine;
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    public List<Vector2> drawPositions;

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
                UpdateLine(currentPosition);
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
    
    void UpdateLine(Vector2 newPosition)
    {
        drawPositions.Add(newPosition);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPosition);
        edgeCollider.points = drawPositions.ToArray();
    }
}
