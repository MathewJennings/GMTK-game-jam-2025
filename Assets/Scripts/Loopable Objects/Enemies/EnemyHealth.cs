using UnityEngine;

public class EnemyHealth : MonoBehaviour, ILoopable
{
    private LevelScriptableObject currentLevel;

    [SerializeField]
    private EnemyDropList enemyDropList;

    [SerializeField]
    private int maxHealth = 1;

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
            transform.localScale *= 0.95f;
            return new LoopResult(0, $"{currentHealth} more", transform.position);
        }
        Destroy(gameObject);
        MaybeDropItem();
        currentLevel.currentPoints += maxHealth;
        return new LoopResult(maxHealth, $"+{maxHealth}pts!", transform.position);
    }

    private void MaybeDropItem()
    {
        if (enemyDropList == null) return;

        int prefabCount = enemyDropList.enemyDropPrefabs.Count;
        int weightCount = enemyDropList.enemyDropWeights.Count;

        if (prefabCount == 0 || prefabCount != weightCount) return;

        float randomWeight = Random.Range(0f, 1f);
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
