using System;
using UnityEngine;

public class InfinityCoinLoopable : MonoBehaviour, ILoopable
{
    [SerializeField]
    private ScoreScriptableObject scoreScriptableObject;

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

    public LoopResult HandleLooped(GameObject line)
    {
        LoopCounter lineCounter = line.GetComponent<LoopCounter>();
        int loopCount = lineCounter.GetCurrentLoopCount();
        int score = isActive ? loopCount : -1 * loopCount;
        scoreScriptableObject.currentScore += score;

        if (!isActive)
        {
            line.GetComponent<LineBreaker>().BreakLine();
        }
        else
        {
            infinityCoinsHandler.HandleGetHit();
        }

        return new LoopResult(score, score > 0 ? $"+{score}" : $"{score}", transform.position);
    }
}
