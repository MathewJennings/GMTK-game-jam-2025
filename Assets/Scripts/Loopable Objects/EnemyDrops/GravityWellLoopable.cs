using UnityEngine;

public class GravityWellLoopable : MonoBehaviour, ILoopable
{
    [SerializeField]
    private GravityWellSuck gravityWellSuckPrefab;

    private bool isPickupScene = false;

    void Awake()
    {
        isPickupScene = GameObject.Find("/SelectPickupManager") != null;
    }

    public LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        Color spriteColor = GetComponentInChildren<SpriteRenderer>().color;
        if (isPickupScene)
        {
            return new LoopResult(0, "Unlocked Gravity Well!", spriteColor, transform.position);
        }
        LogPowerupCollected();

        // Instantiate the gravity well at this object's position
        if (gravityWellSuckPrefab != null)
        {
            GravityWellSuck gravityWell = Instantiate(gravityWellSuckPrefab, transform.position, Quaternion.identity);
            gravityWell.Activate();
        }

        Destroy(gameObject);
        return new LoopResult(0, "Gravity Well activated!", spriteColor, transform.position);
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
