using System;
using UnityEngine;

public class InfinityCoinLoopable : MonoBehaviour, ILoopable
{
    [SerializeField]
    bool isActive;

    private SpriteRenderer spriteRenderer;
    private InfinityCoinsHandler infinityCoinsHandler;

    private readonly Color activeColor = ColorPalette.HotPink;
    private readonly Color inactiveColor = ColorPalette.ElectricCyan;

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
        spriteRenderer.color = isActive ? activeColor : inactiveColor;
    }

    public bool GetIsActive()
    {
        return isActive;
    }

    public void SetIsActive(bool b)
    {
        isActive = b;
        spriteRenderer.color = isActive ? activeColor : inactiveColor;
    }

    public void ToggleIsActive()
    {
        isActive = !isActive;
        spriteRenderer.color = isActive ? activeColor : inactiveColor;
    }

    public LoopResult HandleLooped(GameObject line, float multiplier = 1f)
    {
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
