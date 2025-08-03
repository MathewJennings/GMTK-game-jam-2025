using System.Collections.Generic;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    // 1. Start time of the session
    public float startTime;

    // 2. End Time
    public float endTime;

    // 3. Map of powerup name to number collected
    public Dictionary<string, int> numPowerups = new();

    // 4. Map of boss name to (start time, end time)
    public Dictionary<string, (float startTime, float endTime)> bossTimes = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public string PrintLog()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("===== Game Session Log =====");
        sb.AppendLine($"Time Played: {endTime - startTime:F2} seconds");

        sb.AppendLine("\nPowerups Collected:");
        if (numPowerups.Count == 0)
        {
            sb.AppendLine("  None");
        }
        else
        {
            foreach (var kvp in numPowerups)
            {
                sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
            }
        }

        sb.AppendLine("\nBoss Times:");
        if (bossTimes.Count == 0)
        {
            sb.AppendLine("  None");
        }
        else
        {
            foreach (var kvp in bossTimes)
            {
                sb.AppendLine($"  {kvp.Key}: Duration {(kvp.Value.endTime - kvp.Value.startTime):F2}s");
            }
        }

        sb.AppendLine("============================");
        return sb.ToString();
    }
}
