using UnityEngine;

public class EnemyHealth : MonoBehaviour, ILoopable
{

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
        return new LoopResult(maxHealth, "DEFEATED!", transform.position);
    }
}
