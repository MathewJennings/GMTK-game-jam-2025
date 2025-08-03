using UnityEngine;

public class EnemyHealth : MonoBehaviour, ILoopable
{
    protected LevelScriptableObject currentLevel;
    protected bool isBossMode = false;

    [SerializeField]
    private PickupSelector pickupSelector;

    [SerializeField]
    private EnemyDropPercentages enemyDropPercentages;

    [SerializeField]
    public float maxHealth = 1;

    [SerializeField]
    private bool shrinkOnHit = false;

    public float currentHealth;

    public virtual void AddHealth(float additonalHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth + additonalHealth, 0, maxHealth);
    }

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetCurrentLevel(LevelScriptableObject level)
    {
        currentLevel = level;
    }

    public void SetBossMode(bool isBossMode)
    {
        this.isBossMode = isBossMode;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public virtual LoopResult HandleLooped(GameObject line, float multiplier = 1f)
    {
        Color spriteColor = GetComponentInChildren<SpriteRenderer>().color;
        currentHealth -= 1*multiplier;
        if (currentHealth > 0)
        {
            if (shrinkOnHit)
            {
                transform.localScale *= 0.95f;
            }
            return new LoopResult(0, $"{Mathf.Ceil(currentHealth)} more", spriteColor, transform.position);
        }
        Destroy(gameObject);
        MaybeDropItem();
        if (currentLevel != null)
        {
            currentLevel.currentCorruption -= maxHealth * multiplier;
        }
        string displayText = isBossMode ? "" : $"-{maxHealth} corruption";
        return new LoopResult((int)maxHealth, displayText, spriteColor, transform.position);
    }

    private void MaybeDropItem()
    {
        if (enemyDropPercentages == null) return;

        float dropChance = enemyDropPercentages.dropPercentage;
        float randomWeight = UnityEngine.Random.Range(0f, 1f);

        if (randomWeight <= dropChance)
        {
            GameObject randomDrop = pickupSelector.GetRandomUnlockedPickup();
            if (randomDrop != null)
            {
                Instantiate(randomDrop, transform.position, Quaternion.identity);
            }
        }
    }

    private bool PickupUnlocked(GameObject pickup)
    {
        if (pickupSelector == null)
        {
            return false;
        }

        return pickupSelector.IsPickupUnlocked(pickup);
    }
}
