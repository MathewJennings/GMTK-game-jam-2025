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
    private LoopDetector loopDetector;
    private LoopTextGenerator loopTextGenerator;
    private Vector2 currentCounterTextPosition;


    [Header("Multiplier Settings")]
    [Tooltip("Multiplier per loop completed (default 0.1)")]
    public float perLoopMultiplier = 0.1f;

    void Start()
    {
        mainCamera = Camera.main;
        loopDetector = GetComponent<LoopDetector>();
    }

    public int GetCurrentLoopCount()
    {
        return currentLoopCount;
    }

    public void SetLoopTextGenerator(LoopTextGenerator newLoopTextGenerator)
    {
        loopTextGenerator = newLoopTextGenerator;
    }

    /// <summary>
    /// Increment the loop count and trigger HandleLooped for the loopables.
    /// </summary>
    public void IncrementLoopCountAndHandleLoopables()
    {
        currentLoopCount++;
        List<LoopResult> results = new();

        // First check if there are two infinity coins that share the same parent.
        InfinityCoinsHandler.HandleMultipleInfinityCoins(loopDetector.GetLoopablesInLoop());
        float multiplier = CalculateMultiplier();
        string multiplierText = "x" + multiplier;
        loopTextGenerator.CreateLoopCountText(multiplierText, currentCounterTextPosition + new Vector2(displayOffsetX, displayOffsetY)) ;
        foreach (ILoopable loopable in loopDetector.GetLoopablesInLoop())
        {
            LoopResult result = loopable.HandleLooped(gameObject, multiplier);
            if (!string.IsNullOrEmpty(result.displayText))
            {
                results.Add(result);
            }
        }
        for (int i = 0; i < results.Count; i++)
        {
            Vector2 resultScreenPos = mainCamera.WorldToScreenPoint(results[i].position);
            Vector2 textPosition = resultScreenPos + new Vector2(displayOffsetX, displayOffsetY);
            loopTextGenerator.CreateLoopCountText(results[i].displayText, textPosition, results[i].color);
        }
    }

    public float CalculateMultiplier()
    {
        return 1 + (currentLoopCount - 1) * perLoopMultiplier;
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
