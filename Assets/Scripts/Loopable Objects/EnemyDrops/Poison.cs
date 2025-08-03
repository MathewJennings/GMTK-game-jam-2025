using UnityEngine;

public class Poison : MonoBehaviour, ILoopable
{
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
            return new LoopResult(0, "Unlocked Poison!", spriteColor, transform.position);
        }

        // Affect all LoopableObjects in the scene
        GameObject[] loopableObjects = GameObject.FindGameObjectsWithTag("LoopableObject");
        foreach (GameObject obj in loopableObjects)
        {

            BossHealth bossHealth = obj.GetComponent<BossHealth>();
            EnemyHealth enemyHealth = obj.GetComponent<EnemyHealth>();

            if (bossHealth != null)
            {
                bossHealth.currentHealth /= 2f;
            }
            else if (enemyHealth != null)
            {
                enemyHealth.currentHealth = 1;
                enemyHealth.maxHealth = 1;
            }
        }

        Destroy(gameObject);
        LogPowerupCollected();
        return new LoopResult(0, $"Poison activated!", spriteColor, transform.position);
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
