using UnityEngine;

public class CoinHandler : MonoBehaviour, ILoopable
{
    [SerializeField]
    private ScoreScriptableObject scoreScriptableObject;

    public int HandleLooped(GameObject line)
    {
        LoopCounter lineCounter = line.GetComponent<LoopCounter>();
        int loopCount = lineCounter.GetCurrentLoopCount();
        
        scoreScriptableObject.currentScore += loopCount;
        return loopCount;
    }
}
