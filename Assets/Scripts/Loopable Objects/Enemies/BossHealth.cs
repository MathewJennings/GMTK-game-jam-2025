using UnityEngine;
using UnityEngine.Events;

public class BossHealth : EnemyHealth, ILoopable
{

    [SerializeField]
    private UnityEvent callbackFunction;

    [SerializeField]
    private string deathTextOverride; // Optional override for death text

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public override LoopResult HandleLooped(GameObject line)
    {
        currentHealth--;
        if (currentHealth > 0)
        {
            return new LoopResult(0, $"{currentHealth} more", transform.position);
        }
        Destroy(gameObject);
        callbackFunction?.Invoke();
        currentLevel.hasCompletedBossFight = true;
        string resultText = !string.IsNullOrEmpty(deathTextOverride) ? deathTextOverride : "BOSS DEFEATED!";
        return new LoopResult(maxHealth, resultText, transform.position);
    }
}
