using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int subdivisions = 40;
    public float radius = 1f;

    void Start()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("Line Renderer not assigned!");
            return;
        }

        DrawCircle();
    }

    void DrawCircle()
    {
        lineRenderer.positionCount = subdivisions;
        float angleStep = 2f * Mathf.PI / subdivisions;

        for (int i = 0; i < subdivisions; i++)
        {
            float currentAngle = i * angleStep;
            float x = radius * Mathf.Cos(currentAngle);
            float y = radius * Mathf.Sin(currentAngle);
            Vector3 point = new Vector3(x, y, 0); // Assuming 2D circle on XY plane
            lineRenderer.SetPosition(i, point);
        }
    }
}
