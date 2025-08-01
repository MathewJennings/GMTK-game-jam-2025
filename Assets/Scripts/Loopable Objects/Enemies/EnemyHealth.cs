using System;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, ILoopable
{
    private LevelScriptableObject currentLevel;

    [SerializeField]
    private EnemyDropList enemyDropList;

    [SerializeField]
    private int maxHealth = 1;
    
    [SerializeField]
    private bool shrinkOnHit = false;

    [SerializeField]
    private UnityEvent callbackFunction;

    [SerializeField]
    private string deathTextOverride; // Optional override for death text

    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetCurrentLevel(LevelScriptableObject level)
    {
        currentLevel = level;
    }

    public LoopResult HandleLooped(GameObject line)
    {
        currentHealth--;
        if (currentHealth > 0)
        {
            if (shrinkOnHit)
            {
                transform.localScale *= 0.95f;
            }
            return new LoopResult(0, $"{currentHealth} more", transform.position);
        }
        Destroy(gameObject);
        MaybeDropItem();
        callbackFunction?.Invoke();

        string resultText = !string.IsNullOrEmpty(deathTextOverride) ? deathTextOverride : $"+{maxHealth}pts!";

        if (currentLevel != null)
        {
            currentLevel.currentPoints += maxHealth;
        }
        return new LoopResult(maxHealth, resultText, transform.position);
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
