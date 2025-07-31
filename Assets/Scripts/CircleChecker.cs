using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CircleChecker : MonoBehaviour
{
    private LineRenderer lineRenderer;


    bool IsValidCircle()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            return false;
        } 
        if (lineRenderer.positionCount < 15)
        {
            // Not enough points to form a circle
            Debug.Log("Not enough points to form a circle");
            return false;
        }

        
        // Check if any point beyond point 14 is close enough to any of the first 14 points
        for (int i = 0; i < 14; i++)
        {
            Vector3 referencePoint = lineRenderer.GetPosition(i);
            for (int j = i+14; j < lineRenderer.positionCount; j++)
            {
                Vector3 point = lineRenderer.GetPosition(j);
                if (Vector3.Distance(referencePoint, point) < 0.3f)
                {
                    // Found a point close enough to one of the first 14 points
                    Debug.Log("Circle detected");
                    return true;
                }
            }
        }
        
        return false;
    }
    
    void UpdateCircleColor(bool isValid)
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            // Change the material color based on validity
            Color newColor = isValid ? Color.green : Color.red;
            lineRenderer.material.SetColor("_Color", newColor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            bool isCircle = IsValidCircle();
            UpdateCircleColor(isCircle);
            Debug.Log(isCircle);
        }
    }
}
