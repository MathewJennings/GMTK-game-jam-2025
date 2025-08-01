using UnityEngine;

public class SlimeBoss : MonoBehaviour
{
    public int numTimesHalved = 0;
    [SerializeField]
    private int maxSplits = 3; // Set this in the Inspector

    [SerializeField]
    private float initialMoveSpeed = 1f; // Set this in the Inspector

    // Call this method to halve the parent size and duplicate it
    public void HalveAndDuplicate()
    {
        if (gameObject == null)
        {
            Debug.LogWarning("SlimeBoss: No parent to halve and duplicate.");
            return;
        }

        // Halve the parent's size
        gameObject.transform.localScale *= 0.5f;
        GetComponent<RandomMovement>().moveSpeed *= 2f;

        // Duplicate the parent GameObject
        GameObject duplicate = Instantiate(gameObject, gameObject.transform.position + Vector3.right, gameObject.transform.rotation);

        float currentHealth = GetComponent<BossHealth>().GetCurrentHealth();
        duplicate.GetComponent<SlimeBoss>().numTimesHalved = numTimesHalved;
        GetComponent<BossHealth>().maxHealth = currentHealth;
        duplicate.GetComponent<BossHealth>().currentHealth = currentHealth;
        duplicate.GetComponent<BossHealth>().maxHealth = currentHealth;
        duplicate.GetComponent<RandomMovement>().moveSpeed *= 2f;
    }

    public void CheckAndSplit(float currHealth, float maxHealth)
    {
        if (numTimesHalved < maxSplits && currHealth <= maxHealth * 0.5f)
        {
            numTimesHalved++;
            HalveAndDuplicate();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<RandomMovement>().moveSpeed = initialMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
