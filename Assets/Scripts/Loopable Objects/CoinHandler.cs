using UnityEngine;

public class CoinHandler : MonoBehaviour, ILoopable
{
    [SerializeField]
    private ScoreScriptableObject scoreScriptableObject;

    public int HandleLooped(int loopCount)
    {
        scoreScriptableObject.currentScore += loopCount;
        return loopCount;
    }
}
