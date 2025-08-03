using UnityEngine;

public class GravityWellLoopable : MonoBehaviour, ILoopable
{
    [SerializeField]
    private GravityWellSuck gravityWellSuck;
    
    private bool isPickupScene = false;
    
    void Awake()
    {
        isPickupScene = GameObject.Find("/SelectPickupManager") != null;
    }
    public LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        if (isPickupScene)
        {
            return new LoopResult(0, "Unlocked gravity well!", Color.purple, transform.position);
        }
        LogPowerupCollected();
        gravityWellSuck.Activate();
        Destroy(gameObject);
        return new LoopResult(0, "Gravity Well activated!", Color.purple, transform.position);
    }

    private void LogPowerupCollected()
    {
        LogManager logManager = FindFirstObjectByType<LogManager>();
        if (logManager != null)
        {
            string powerupName = this.GetType().Name;
            if (logManager.numPowerups.ContainsKey(powerupName))
            {
                logManager.numPowerups[powerupName]++;
            }
            else
            {
                logManager.numPowerups[powerupName] = 1;
            }
        }
    }
}
