using UnityEngine;

public class EnemyHealth : MonoBehaviour, ILoopable
{
    [SerializeField]
    private ScoreScriptableObject scoreScriptableObject;

    [SerializeField]
    private EnemyDropList enemyDropList;

    [SerializeField]
    private int maxHealth = 1;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public LoopResult HandleLooped(GameObject line)
    {
        currentHealth--;
        if (currentHealth > 0)
        {
            return new LoopResult(0, $"{currentHealth} more", transform.position);
        }
        Destroy(gameObject);
        MaybeDropItem();
        
        scoreScriptableObject.currentScore += maxHealth;
        return new LoopResult(maxHealth, $"+{maxHealth}pts!", transform.position);
    }

    private void MaybeDropItem()
    {
        if (enemyDropList != null && enemyDropList.enemyDropPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, enemyDropList.enemyDropPrefabs.Count);
            GameObject randomDrop = enemyDropList.enemyDropPrefabs[randomIndex];
            Instantiate(randomDrop, transform.position, Quaternion.identity);
        }
    }
}
