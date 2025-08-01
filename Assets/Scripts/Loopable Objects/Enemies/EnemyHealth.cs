using System;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, ILoopable
{
    protected LevelScriptableObject currentLevel;

    [SerializeField]
    private EnemyDropList enemyDropList;

    [SerializeField]
    protected float maxHealth = 1;

    [SerializeField]
    private bool shrinkOnHit = false;

    protected float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetCurrentLevel(LevelScriptableObject level)
    {
        currentLevel = level;
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
        currentHealth -= 1*multiplier;
        if (currentHealth > 0)
        {
            if (shrinkOnHit)
            {
                transform.localScale *= 0.95f;
            }
            return new LoopResult(0, $"{Mathf.Ceil(currentHealth)} more", Color.red, transform.position);
        }
        Destroy(gameObject);
        MaybeDropItem();
        if (currentLevel != null)
        {
            currentLevel.currentPoints += maxHealth * multiplier;
        }
        return new LoopResult((int)maxHealth, $"+{maxHealth}pts!", Color.red, transform.position);
    }

    private void MaybeDropItem()
    {
        if (enemyDropList == null) return;

        int prefabCount = enemyDropList.enemyDropPrefabs.Count;
        int weightCount = enemyDropList.enemyDropWeights.Count;

        if (prefabCount == 0 || prefabCount != weightCount) return;

        float randomWeight = UnityEngine.Random.Range(0f, 1f);

        float cumulativeWeight = 0f;

        for (int i = 0; i < weightCount; i++)
        {
            cumulativeWeight += enemyDropList.enemyDropWeights[i];
            if (randomWeight <= cumulativeWeight)
            {
                GameObject randomDrop = enemyDropList.enemyDropPrefabs[i];
                Instantiate(randomDrop, transform.position, Quaternion.identity);
                break;
            }
        }
    }
}
