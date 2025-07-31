using System.Collections.Generic;
using UnityEngine;

public class LoopDetector : MonoBehaviour
{
    [SerializeField]
    float minimumLoopArea = 1.0f;

    // Check if the very last point in drawPositions is close to any other point.
    // Ignore the 15 latest points to avoid false positives.
    public bool CreatedLoop(List<Vector2> drawPositions, List<bool> drawValidForLoops)
    {
        if (drawPositions.Count < 15) return false;

        Vector2 lastPoint = drawPositions[^1];
        Vector2 secondLastPoint = drawPositions[^2];
        // Start checking at 14 from the end.
        for (int i = drawPositions.Count - 14; i >= 1; i--)
        {
            // Return false if this point was already used in a previous loop.
            if (!drawValidForLoops[i]) return false;

            // Check if last line segment intersects with any previous segment.
            //if (Vector2.Distance(lastPoint, drawPositions[i]) < 0.1f)
            if (DoLineSegmentsIntersect(lastPoint, secondLastPoint, drawPositions[i], drawPositions[i-1]))
            {
                // An intersection was found. Invalidate all points up until now.
                // Keep the last ~10 valid so that those can still be considered for future loops.
                for (int j = drawValidForLoops.Count - 10; j >= 0; j--)
                {
                    // End early if we hit invalid point. Rest is already marked as invalid.
                    if (!drawValidForLoops[j]) break;
                    drawValidForLoops[j] = false;
                }

                // Check if the loop with all the points involved has a large enough area.
                float area = CalculatePolygonArea(
                    drawPositions.GetRange(i, drawPositions.Count - i));
                if (area > minimumLoopArea)
                {
                    ProcessObjectsInLoop(drawPositions.GetRange(i, drawPositions.Count - i));
                    return true;
                }

                return false;
            }
        }

        return false;
    }

    // Check if two line segments intersect
    private bool DoLineSegmentsIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        // Helper function to find the orientation of three points
        int Orientation(Vector2 a, Vector2 b, Vector2 c)
        {
            float val = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
            if (Mathf.Abs(val) < Mathf.Epsilon) return 0; // Collinear
            return (val > 0) ? 1 : 2; // Clockwise or Counterclockwise
        }

        // Check if point c lies on segment ab
        bool OnSegment(Vector2 a, Vector2 b, Vector2 c)
        {
            return c.x <= Mathf.Max(a.x, b.x) && c.x >= Mathf.Min(a.x, b.x) &&
                   c.y <= Mathf.Max(a.y, b.y) && c.y >= Mathf.Min(a.y, b.y);
        }

        int o1 = Orientation(p1, q1, p2);
        int o2 = Orientation(p1, q1, q2);
        int o3 = Orientation(p2, q2, p1);
        int o4 = Orientation(p2, q2, q1);

        // General case: segments intersect if orientations differ
        if (o1 != o2 && o3 != o4) return true;

        // Special cases: check if points are collinear and lie on the segment
        if (o1 == 0 && OnSegment(p1, q1, p2)) return true;
        if (o2 == 0 && OnSegment(p1, q1, q2)) return true;
        if (o3 == 0 && OnSegment(p2, q2, p1)) return true;
        if (o4 == 0 && OnSegment(p2, q2, q1)) return true;

        return false; // No intersection
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

    private void ProcessObjectsInLoop(List<Vector2> polygonPoints)
    {
        List<GameObject> objectsInsidePolygon = GetGameObjectsInsidePolygon(polygonPoints);

        if (objectsInsidePolygon.Count > 0)
        {
            foreach (GameObject obj in objectsInsidePolygon)
            {
                // Get the ILoopable component from the object and call HandleLooped
                ILoopable loopable = obj.GetComponent<ILoopable>();
                if (loopable != null)
                {
                    loopable.HandleLooped();
                }
            }
        }
    }

    private List<GameObject> GetGameObjectsInsidePolygon(List<Vector2> polygonPoints)
    {
        List<GameObject> objectsInsidePolygon = new List<GameObject>();

        // Get all game objects tagged with "loopable"
        GameObject[] loopableObjects = GameObject.FindGameObjectsWithTag("LoopableObject");

        foreach (GameObject obj in loopableObjects)
        {
            // Check if the object's position is inside the polygon
            if (IsPointInPolygon(obj.transform.position, polygonPoints))
            {
                objectsInsidePolygon.Add(obj);
            }
        }

        return objectsInsidePolygon;
    }

    // Ray casting algorithm to check if a point is inside a polygon
    private bool IsPointInPolygon(Vector2 point, List<Vector2> polygonPoints)
    {
        int count = polygonPoints.Count;
        bool inside = false;

        for (int i = 0, j = count - 1; i < count; j = i++)
        {
            if ((polygonPoints[i].y > point.y) != (polygonPoints[j].y > point.y) &&
                (point.x < (polygonPoints[j].x - polygonPoints[i].x) * (point.y - polygonPoints[i].y) / (polygonPoints[j].y - polygonPoints[i].y) + polygonPoints[i].x))
            {
                inside = !inside;
            }
        }

        return inside;
    }
}
