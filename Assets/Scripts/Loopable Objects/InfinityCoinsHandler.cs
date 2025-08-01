using System.Collections.Generic;
using UnityEngine;

public class InfinityCoinsHandler : MonoBehaviour
{
    [SerializeField]
    private float minRotationSpeed = 30f; // Minimum rotation speed in degrees per second

    [SerializeField]
    private float maxRotationSpeed = 90f; // Maximum rotation speed in degrees per second
    
    [SerializeField]
    private float rotationSpeedHitIncrease = 5f; // How much speed increases with each hit

    [SerializeField]
    private float rotationChangeInterval = 5f; // Time in seconds to change rotation speed

    List<InfinityCoinLoopable> children;
    InfinityCoinLoopable activeChild;

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

        children = new List<InfinityCoinLoopable>();
        activeChild = null;

        // Collect all children and find the active one
        foreach (Transform child in transform)
        {
            InfinityCoinLoopable loopable = child.GetComponent<InfinityCoinLoopable>();
            if (loopable != null)
            {
                children.Add(loopable);
                if (loopable.GetIsActive())
                {
                    activeChild = loopable;
                }
            }
        }
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
        }
    }

    private void ChangeRotationSpeed()
    {
        // Randomize the rotation speed.
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        // Randomly reverse the direction
        if (Random.value < 0.5f)
        {
            rotationSpeed = -rotationSpeed;
        }

        timeSinceLastChange = 0f;
    }

    public void HandleGetHit()
    {
        // Increase min and max rotation speed and change speed.
        float oldRotationSpeed = rotationSpeed;
        minRotationSpeed += rotationSpeedHitIncrease;
        maxRotationSpeed += rotationSpeedHitIncrease;
        ChangeRotationSpeed();
        if (Mathf.Sign(rotationSpeed) == Mathf.Sign(oldRotationSpeed))
        {
            rotationSpeed = -rotationSpeed;
        }

        SetRandomActiveChild();
    }
    
    private void SetRandomActiveChild()
    {
        // If no children or only one child exists, do nothing
        if (children.Count <= 1) return;

        // Deactivate the currently active child
        if (activeChild != null)
        {
            activeChild.SetIsActive(false);
        }

        // Select a random different child to activate
        InfinityCoinLoopable newActiveChild;
        do
        {
            newActiveChild = children[Random.Range(0, children.Count)];
        } while (newActiveChild == activeChild);

        activeChild = newActiveChild;
        activeChild.SetIsActive(true);
    }
}