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
    /// Increment the loop count and trigger HandleLooped for the loopables.
    /// </summary>
    public void IncrementLoopCountAndHandleLoopables()
    {
        currentLoopCount++;
        List<LoopResult> results = new();
        foreach (ILoopable loopable in loopDetector.GetLoopablesInLoop())
        {
            LoopResult result = loopable.HandleLooped(gameObject);
            if (!string.IsNullOrEmpty(result.displayText))
            {
                results.Add(result);
            }
        }
        float textSpacing = 50f;
        for (int i = 0; i < results.Count; i++)
        {
            Vector2 resultScreenPos = mainCamera.WorldToScreenPoint(results[i].position);
            Vector2 weightedCenterPosition = (2f * currentCounterTextPosition + 3f * resultScreenPos) / 5f;
            float totalSize = (results.Count - 1) * textSpacing;
            Vector2 stackOffset = new(0, (i * textSpacing) - (totalSize * 0.5f));
            Vector2 textPosition = weightedCenterPosition + stackOffset;
            canvas.GetComponent<LoopTextGenerator>().CreateLoopCountText(results[i].displayText, textPosition);
        }
    }

    /// <summary>
    /// Update the position where the tip of the line cursor currently is.
    /// </summary>
    /// <param name="worldPosition">The current world position of the line drawing.</param>
    public void UpdateLineTipPosition(Vector2 worldPosition)
    {
        currentCounterTextPosition = mainCamera.WorldToScreenPoint(worldPosition);
    }
}
