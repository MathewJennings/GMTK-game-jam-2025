using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopCounter : MonoBehaviour
{

    public float displayOffsetX = 100f; // Offset from the current mouse position
    public float displayOffsetY = 50f;
    private int currentLoopCount = 0;
    private Camera mainCamera;
    private Canvas canvas;
    private Vector2 currentCounterTextPosition;
    private LoopDetector loopDetector;

    void Start()
    {
        mainCamera = Camera.main;
        loopDetector = GetComponent<LoopDetector>();
    }

    public int GetCurrentLoopCount()
    {
        return currentLoopCount;
    }

    public void SetCanvas(Canvas newCanvas)
    {
        canvas = newCanvas;
    }

    /// <summary>
    /// Increment the loop count and create a new loop count text to display it.
    /// </summary>
    public void IncrementLoopCountAndHandleLoopables()
    {
        currentLoopCount++;
        int totalScoreChange = 0;
        foreach (ILoopable loopable in loopDetector.GetLoopablesInLoop())
        {
            totalScoreChange += loopable.HandleLooped(gameObject);
        }
        string text = (totalScoreChange >= 0 ? "+" : "") + totalScoreChange.ToString();
        canvas.GetComponent<LoopTextGenerator>().CreateLoopCountText(text, currentCounterTextPosition);
    }

    /// <summary>
    /// Update the position where we will display the counter for the next loop.
    /// </summary>
    /// <param name="worldPosition">The current world position of the line drawing.</param>

    public void UpdateCounterTextPosition(Vector2 worldPosition)
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
        screenPosition.x += displayOffsetX;
        screenPosition.y += displayOffsetY;
        currentCounterTextPosition = screenPosition;
    }
}
