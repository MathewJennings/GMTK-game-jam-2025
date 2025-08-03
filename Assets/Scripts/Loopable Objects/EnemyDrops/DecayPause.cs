using NUnit.Framework.Constraints;
using UnityEngine;

public class DecayPause : MonoBehaviour, ILoopable
{
    [SerializeField]
    private float duration = 5f;

    private WaveProgressBar waveProgressBar;
    private BossProgressBar bossProgressBar;

    private bool isPickupScene = false;

    void Awake()
    {
        GameObject canvas = GameObject.Find("/UI Canvas");
        if (canvas != null)
        {
            waveProgressBar = canvas.transform.Find("ProgressBarBackground").GetComponent<WaveProgressBar>();
            bossProgressBar = canvas.transform.Find("BossBarBackground").GetComponent<BossProgressBar>();
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
        Color spriteColor = GetComponentInChildren<SpriteRenderer>().color;
        if (isPickupScene)
        {
            return new LoopResult(0, "Unlocked Halt Corruption!", spriteColor, transform.position);
        }

        LogPowerupCollected();
        waveProgressBar.PauseCorruption(duration);
        bossProgressBar.PauseCorruption(duration);
        Destroy(gameObject);
        return new LoopResult(0, "Corruption halted!", spriteColor, transform.position);
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
