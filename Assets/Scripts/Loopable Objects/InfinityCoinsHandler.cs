using System.Collections.Generic;
using UnityEngine;

public class InfinityCoinsHandler : MonoBehaviour
{
    [SerializeField]
    private float maxRotationSpeed = 45f; // Maximum rotation speed in degrees per second
    
    [SerializeField]
    private float rotationChangeInterval = 5f; // Time in seconds to change rotation speed
    
    private float rotationSpeed;
    private float timeSinceLastChange;

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

    void Start()
    {
        // Initialize with a random rotation speed
        ChangeRotationSpeed();
    }

    void Update()
    {
        // Rotate the transform on the z-axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Update the timer
        timeSinceLastChange += Time.deltaTime;

        // Change rotation speed and direction every 5 seconds
        if (timeSinceLastChange >= rotationChangeInterval)
        {
            ChangeRotationSpeed();
            timeSinceLastChange = 0f;
        }
    }

    private void ChangeRotationSpeed()
    {
        // Randomize the rotation speed and direction
        rotationSpeed = Random.Range(-1 * maxRotationSpeed, maxRotationSpeed);
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
