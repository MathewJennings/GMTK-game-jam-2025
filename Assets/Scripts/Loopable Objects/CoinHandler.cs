using UnityEngine;

public class CoinHandler : MonoBehaviour, ILoopable
{
    [SerializeField]
    private LevelManager levelManager;

    void Awake()
    {
        if (levelManager == null)
        {
            Debug.LogWarning("CoinHandler: Missing reference to LevelManager.");
        }
    }

    public LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        LoopCounter lineCounter = line.GetComponent<LoopCounter>();
        int loopCount = lineCounter.GetCurrentLoopCount();
        levelManager.currentLevel.currentCorruption -= loopCount;
        return new LoopResult(loopCount, $"+{loopCount}", Color.yellow, transform.position);
    }
}
