using NUnit.Framework.Constraints;
using UnityEngine;

public class DecayPause : MonoBehaviour, ILoopable
{
    [SerializeField]
    private float duration = 5f;

    private WaveProgressBar waveProgressBar;

    private bool isPickupScene = false;

    void Awake()
    {
        GameObject canvas = GameObject.Find("/UI Canvas");
        if (canvas != null)
        {
            waveProgressBar = canvas.transform.Find("ProgressBarBackground").GetComponent<WaveProgressBar>();
        }
        if (waveProgressBar == null)
        {
            Debug.LogWarning("DecayPause: Missing reference to WaveProgressBar.");
            Destroy(gameObject);
        }

        isPickupScene = GameObject.Find("/SelectPickupManager") != null;
    }

    public LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        if (isPickupScene)
        {
            return new LoopResult(0, "Unlocked decay pause!", new Color(100f / 255f, 255f / 255f, 255f / 255f), transform.position);
        }

        LogPowerupCollected();
        waveProgressBar.PauseCorruption(duration);
        Destroy(gameObject);
        return new LoopResult(0, "Corruption paused!", new Color(100f / 255f, 255f / 255f, 255f / 255f), transform.position);
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
