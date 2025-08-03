using System;
using UnityEngine;
using UnityEngine.Events;

public class BossHealth : EnemyHealth, ILoopable
{
    [SerializeField]
    private UnityEvent onDeath;

    [SerializeField]
    private UnityEvent<float, float> onDamage;

    [SerializeField]
    private string deathTextOverride; // Optional override for death text

    private Action NotifyBossDefeated;

    private float bossStartTime;
    private float bossEndTime;

    public void SetNotifyBossDefeated(Action notifyBossDefeated)
    {
        NotifyBossDefeated = notifyBossDefeated;
    }

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        bossStartTime = Time.time;
    }

    public override LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        currentHealth -= 1 * multiplier;
        if (currentHealth > 0)
        {
            onDamage?.Invoke(currentHealth, maxHealth);
            Color spriteColor = GetComponentInChildren<SpriteRenderer>().color;
            return new LoopResult(0, $"{Mathf.Ceil(currentHealth)} more", spriteColor, transform.position);
        }
        return OnDefeatBoss();
    }

    protected LoopResult OnDefeatBoss()
    {
        bossEndTime = Time.time;

        // Log to LogManager2
        LogManager logManager = FindFirstObjectByType<LogManager>();
        if (logManager != null)
        {
            Debug.Log($"Boss defeated: {gameObject.name} at {bossEndTime:F2}, {bossStartTime:F2}");
            string bossName = gameObject.GetComponent<BossName>()?.bossName?.Replace("\n", " ") ?? gameObject.name;
            logManager.bossTimes[bossName] = (bossStartTime, bossEndTime);
        }

        Destroy(gameObject);
        onDeath?.Invoke();
        NotifyBossDefeated();
        string resultText = !string.IsNullOrEmpty(deathTextOverride) ? deathTextOverride : "BOSS DEFEATED!";
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Color spriteColor = spriteRenderer != null ? spriteRenderer.color : ColorPalette.BrightYellow;
        return new LoopResult((int)maxHealth, resultText, spriteColor, transform.position);
    }
}
