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

    public int HandleLooped(GameObject line)
    {
        currentHealth--;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            return maxHealth;
        }
        return 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
