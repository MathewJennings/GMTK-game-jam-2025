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

    public LoopResult HandleLooped(GameObject line)
    {
        LoopCounter lineCounter = line.GetComponent<LoopCounter>();
        int loopCount = lineCounter.GetCurrentLoopCount();
        levelManager.currentLevel.currentPoints += loopCount;
        return new LoopResult(loopCount, $"+{loopCount}", transform.position);
    }
}
