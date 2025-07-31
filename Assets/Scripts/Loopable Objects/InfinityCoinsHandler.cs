using UnityEngine;

public class InfinityCoinsHandler : MonoBehaviour
{
    public void HandleLoopedCoin()
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
