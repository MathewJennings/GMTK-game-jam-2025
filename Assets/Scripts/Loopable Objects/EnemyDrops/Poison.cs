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
            return new LoopResult(0, "Unlocked Poison Blast!", spriteColor, transform.position);
        }

        // Affect all LoopableObjects in the scene
        GameObject[] loopableObjects = GameObject.FindGameObjectsWithTag("LoopableObject");
        foreach (GameObject obj in loopableObjects)
        {

            BossHealth bossHealth = obj.GetComponent<BossHealth>();
            EnemyHealth enemyHealth = obj.GetComponent<EnemyHealth>();
            InfinityCoinsHandler infinityCoinsHandler = obj.GetComponent<InfinityCoinsHandler>();
            SlimeChild slimeChild = obj.GetComponent<SlimeChild>();


            if (slimeChild != null)
            {
                int numAttacks = Mathf.RoundToInt(slimeChild.health / 2f);
                for (int i = 0; i < numAttacks; i++)
                {
                    slimeChild.HandleLooped(null, 1);
                    if (slimeChild.health <= 0)
                    {
                        break;
                    }
                }
            }
            else if (infinityCoinsHandler != null)
            {
                infinityCoinsHandler.currentHealth = infinityCoinsHandler.currentHealth / 2f;
            }
            else if (bossHealth != null)
            {
                int numAttacks = Mathf.RoundToInt(bossHealth.currentHealth / 2f);
                for (int i = 0; i < numAttacks; i++)
                {
                    bossHealth.HandleLooped(null, 1);
                    if (bossHealth.currentHealth <= 0)
                    {
                        break;
                    }
                }
            }
            else if (enemyHealth != null)
            {
                enemyHealth.currentHealth = 1;
                enemyHealth.maxHealth = 1;
            }
        }

        Destroy(gameObject);
        LogPowerupCollected();
        return new LoopResult(0, $"Poison Blast activated!", spriteColor, transform.position);
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
