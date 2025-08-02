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

    public void SetNotifyBossDefeated(Action notifyBossDefeated)
    {
        NotifyBossDefeated = notifyBossDefeated;
    }

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public override LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        currentHealth -= 1 * multiplier;
        if (currentHealth > 0)
        {
            onDamage?.Invoke(currentHealth, maxHealth);
            return new LoopResult(0, $"{Mathf.Ceil(currentHealth)} more", Color.red, transform.position);
        }
        return OnDefeatBoss();
    }

    protected LoopResult OnDefeatBoss()
    {
        Destroy(gameObject);
        onDeath?.Invoke();
        NotifyBossDefeated();
        string resultText = !string.IsNullOrEmpty(deathTextOverride) ? deathTextOverride : "BOSS DEFEATED!";
        return new LoopResult((int)maxHealth, resultText, Color.red, transform.position);
    }
}
