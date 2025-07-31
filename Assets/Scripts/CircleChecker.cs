using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CircleChecker : MonoBehaviour
{
    private LineRenderer lineRenderer;


    // Detect if there is a loop originating from the start index
    int CountValidLoops()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            return 0;
        }

        int loopCount = 0;
        int start = 0;
        
        // If there are at least 15 points after start, do another loop check from start.
        while (lineRenderer.positionCount - start >= 15)
        {
            bool loopFound = false;
            for (int i = start; i < start+14; i++)
            {
                Vector3 referencePoint = lineRenderer.GetPosition(i);
                for (int j = i+14; j < lineRenderer.positionCount; j++)
                {
                    Vector3 point = lineRenderer.GetPosition(j);
                    if (Vector3.Distance(referencePoint, point) < 0.3f)
                    {
                        // Increment the loop count, and do another iteration starting at j.
                        loopCount++;
                        loopFound = true;
                        start = j;
                        break;
                    }
                }

                if (loopFound)
                {
                    break;
                }
            }
            // If no loop was found, break the loop.
            if (!loopFound)
            {
                break;
            }
        }
        return loopCount;
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
            int loopCount = CountValidLoops();
            UpdateCircleColor(loopCount > 0);
            Debug.Log(loopCount);
        }
    }
}
