using UnityEngine;

public class Ghost : MonoBehaviour, ILoopable
{
    [SerializeField]
    private float ghostDuration = 3f; // Configurable in the Inspector

    private bool isPickupScene = false;

    void Awake()
    {
        isPickupScene = GameObject.Find("/SelectPickupManager") != null;
    }

    public LoopResult HandleLooped(GameObject line, float multiplier = 1f)
    {
        Color spriteColor = GetComponentInChildren<SpriteRenderer>().color;
        if (isPickupScene)
        {
            return new LoopResult(0, "Unlocked Ghost Line!", spriteColor, transform.position);
        }

        LogPowerupCollected();
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");

        foreach (GameObject lineObj in lines)
        {
            LineBreaker lineBreaker = lineObj.GetComponent<LineBreaker>();
            if (lineBreaker != null)
            {
                lineBreaker.SetGhostMode(ghostDuration);
            }

            // Add LineGhostColor component if not already present
            LineGhostColor ghostColor = lineObj.GetComponent<LineGhostColor>();
            if (ghostColor == null)
            {
                ghostColor = lineObj.AddComponent<LineGhostColor>();
            }
            ghostColor.GhostifyLine(ghostDuration);
        }

        // Find the "Line Spawner" GameObject and call SetGhostMode on it
        GameObject lineSpawner = GameObject.Find("Line Spawner");
        if (lineSpawner != null)
        {
            var spawnLine = lineSpawner.GetComponent<SpawnLine>();
            if (spawnLine != null)
            {
                spawnLine.SetGhostMode(ghostDuration);
            }
        }
        else
        {
            Debug.Log("Line Spawner not found or SpawnLine component missing.");
        }

        Destroy(gameObject);
        return new LoopResult(0, "Ghost Line activated!", spriteColor, transform.position);
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
