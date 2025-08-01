using System;
using UnityEngine;

public class InfinityCoinLoopable : MonoBehaviour, ILoopable
{
    [SerializeField]
    bool isActive;

    private SpriteRenderer spriteRenderer;
    private InfinityCoinsHandler infinityCoinsHandler;

    void Awake()
    {
        infinityCoinsHandler = GetComponentInParent<InfinityCoinsHandler>();

        // Get the child object named "Sprite" and its SpriteRenderer
        Transform spriteChild = transform.Find("Sprite");
        if (spriteChild != null)
        {
            spriteRenderer = spriteChild.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer not found on the child object.");
            }
        }
        spriteRenderer.color = isActive ? Color.red : Color.purple;
    }

    public bool GetIsActive()
    {
        return isActive;
    }

    public void SetIsActive(bool b)
    {
        isActive = b;
        spriteRenderer.color = isActive ? Color.red : Color.purple;
    }

    public void ToggleIsActive()
    {
        isActive = !isActive;
        spriteRenderer.color = isActive ? Color.red : Color.purple;
    }

    public LoopResult HandleLooped(GameObject line, float multiplier = 1f)
    {
        LoopCounter lineCounter = line.GetComponent<LoopCounter>();
        int loopCount = lineCounter.GetCurrentLoopCount();
        int points = isActive ? loopCount : -1 * loopCount;

        if (!isActive)
        {
            line.GetComponent<LineBreaker>().BreakLine();
            LoopResult result = infinityCoinsHandler.HandleIncorrectLoop(line, spriteRenderer.color, multiplier);
            return result;
        }
        else
        {
            LoopResult result = infinityCoinsHandler.HandleGetHit(line, spriteRenderer.color, multiplier);
            return result;
        }
    }
}
