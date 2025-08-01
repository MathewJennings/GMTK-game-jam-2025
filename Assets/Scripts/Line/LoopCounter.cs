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

        foreach (ILoopable loopable in loopDetector.GetLoopablesInLoop())
        {
            LoopResult result = loopable.HandleLooped(gameObject);
            if (!string.IsNullOrEmpty(result.displayText))
            {
                results.Add(result);
            }
        }
        for (int i = 0; i < results.Count; i++)
        {
            Vector2 resultScreenPos = mainCamera.WorldToScreenPoint(results[i].position);
            Vector2 textPosition = resultScreenPos + new Vector2(displayOffsetX, displayOffsetY);
            loopTextGenerator.CreateLoopCountText(results[i].displayText, textPosition);
        }
    }
}
