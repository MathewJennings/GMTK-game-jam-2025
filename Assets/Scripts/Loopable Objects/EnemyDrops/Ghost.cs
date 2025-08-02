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
        if (isPickupScene)
        {
            return new LoopResult(0, "Unlocked ghost mode!", Color.grey, transform.position);
        }
        
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
        
        foreach (GameObject lineObj in lines)
        {
            LineBreaker lineBreaker = lineObj.GetComponent<LineBreaker>();
            if (lineBreaker != null)
            {
                lineBreaker.SetGhostMode(ghostDuration);
            }
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
        } else
        {
            Debug.Log("Line Spawner not found or SpawnLine component missing.");
        }

        Destroy(gameObject);
        return new LoopResult(0, "Ghost mode activated!", Color.grey, transform.position);
    }
}
