using UnityEngine;

public class CoinHandler : MonoBehaviour, ILoopable
{
    public void HandleLooped()
    {
        Debug.Log("I'm a coin!");
    }
}
