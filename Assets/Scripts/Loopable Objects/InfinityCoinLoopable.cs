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
        spriteRenderer.color = isActive ? Color.green : Color.red;
    }
    
    public int HandleLooped(int loopCount)
    {
        int actualCount = isActive ? loopCount : -1 * loopCount;
        
        infinityCoinsHandler.HandleLoopedCoin();

        return actualCount;
    }
    
    public void ToggleIsActive()
    {
        isActive = !isActive;
        spriteRenderer.color = isActive ? Color.green : Color.red;
    }
}
