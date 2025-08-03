using Unity.VisualScripting;
using UnityEngine;

public class DamageMultiplier : MonoBehaviour, ILoopable
{
    [SerializeField]
    private float bonusMultiplier = 1f;
    
    [SerializeField]
    private float duration = 5f;

    private bool isPickupScene = false;
    private SpawnLine spawnLine;

    void Awake()
    {
        isPickupScene = GameObject.Find("/SelectPickupManager") != null;
        spawnLine = GameObject.Find("/Line Spawner").GetComponent<SpawnLine>();
    }

    public LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        Color spriteColor = GetComponentInChildren<SpriteRenderer>().color;
        if (isPickupScene)
        {
            return new LoopResult(0, "Unlocked Damage Multiplier!", spriteColor, transform.position);
        }

        float newMult = spawnLine.TemporarilyAddBonusMultiplier(bonusMultiplier, duration);
        Destroy(gameObject);
        LogPowerupCollected();
        return new LoopResult(0, $"{newMult}x Damage activated!", spriteColor, transform.position);
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
