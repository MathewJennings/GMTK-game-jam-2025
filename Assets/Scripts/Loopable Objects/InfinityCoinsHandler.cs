using System.Collections.Generic;
using UnityEngine;

public class InfinityCoinsHandler : MonoBehaviour
{
    public static void HandleMultipleInfinityCoins(List<ILoopable> loopables)
    {
        // If there are two infinity coin objects in the list that share the same parent, remove the one that is
        // active from loopables.
        Dictionary<Transform, InfinityCoinLoopable> parentToLoopableMap = new();

        for (int i = loopables.Count - 1; i >= 0; i--)
        {
            // Try casting to InfinityCoinLoopable, and skip it if it fails.
            InfinityCoinLoopable infinityCoinLoopable = loopables[i] as InfinityCoinLoopable;
            if (infinityCoinLoopable == null) continue;

            Transform parent = infinityCoinLoopable.transform.parent;

            if (parent != null)
            {
                if (parentToLoopableMap.ContainsKey(parent))
                {
                    // If a duplicate parent is found, remove the active one
                    InfinityCoinLoopable existingInfinityCoin = parentToLoopableMap[parent];
                    if (existingInfinityCoin.GetIsActive())
                    {
                        loopables.Remove(existingInfinityCoin);
                    }
                    else
                    {
                        loopables.Remove(loopables[i]);
                    }
                }
                else
                {
                    parentToLoopableMap[parent] = infinityCoinLoopable;
                }
            }
        }
    }
    
    public void ToggleActiveCoins()
    {
        foreach (Transform child in transform)
        {
            InfinityCoinLoopable loopable = child.GetComponent<InfinityCoinLoopable>();
            if (loopable != null)
            {
                loopable.ToggleIsActive();
            }
        }
    }
}
