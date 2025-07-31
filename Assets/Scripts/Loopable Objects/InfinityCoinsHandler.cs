using UnityEngine;

public class InfinityCoinsHandler : MonoBehaviour
{
    public void HandleLoopedCoin(bool isActive)
    {
        if (isActive)
        {
            // Logic for handling the looped coin when it is active
            Debug.Log("Looped coin is active.");
        }
        else
        {
            // Logic for handling the looped coin when it is inactive
            Debug.Log("Looped coin is inactive.");
        }
        
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
