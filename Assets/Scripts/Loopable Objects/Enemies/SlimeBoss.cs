using UnityEngine;

public class SlimeBoss : BossHealth
{
    [SerializeField]
    private GameObject slimeChildPrefab; // Set this in the Inspector
    
    public int numTimesHalved = 0;
    [SerializeField]
    private int maxSplits = 3; // Set this in the Inspector

    [SerializeField]
    private float initialMoveSpeed = 1f; // Set this in the Inspector

    // Override this to do nothing on looped result. That is handled by the children slimes.
    public override LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        return new LoopResult(0, null, Color.red, transform.position);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the first giant slime.
        GameObject slime = Instantiate(slimeChildPrefab, Vector3.zero, Quaternion.identity);
        // slime.transform.SetParent(transform);
        slime.GetComponent<RandomMovement>().moveSpeed = initialMoveSpeed;
        SlimeChild slimeChild = slime.GetComponent<SlimeChild>();
        if (slimeChild != null)
        {
            slimeChild.SetSlimeBoss(this);
            slimeChild.SetMaxHealth(maxHealth);
            slimeChild.SetHealth(maxHealth);
            slimeChild.SetSplitsRemaining(maxSplits);
            slimeChild.SetMoveSpeed(initialMoveSpeed);
            slimeChild.GetComponent<RandomMovement>().moveSpeed = initialMoveSpeed;
        }
    }

    // // Call this method to halve the parent size and duplicate it
    // public void HalveAndDuplicate()
    // {
    //     if (gameObject == null)
    //     {
    //         Debug.LogWarning("SlimeBoss: No parent to halve and duplicate.");
    //         return;
    //     }
    //
    //     // Halve the parent's size
    //     gameObject.transform.localScale *= 0.5f;
    //     GetComponent<RandomMovement>().moveSpeed *= 2f;
    //
    //     // Duplicate the parent GameObject
    //     GameObject duplicate = Instantiate(gameObject, gameObject.transform.position + Vector3.right, gameObject.transform.rotation);
    //
    //     float currentHealth = GetComponent<BossHealth>().GetCurrentHealth();
    //     duplicate.GetComponent<SlimeBoss>().numTimesHalved = numTimesHalved;
    //     GetComponent<BossHealth>().maxHealth = currentHealth;
    //     duplicate.GetComponent<BossHealth>().currentHealth = currentHealth;
    //     duplicate.GetComponent<BossHealth>().maxHealth = currentHealth;
    //     duplicate.GetComponent<RandomMovement>().moveSpeed *= 2f;
    // }

    // public void CheckAndSplit(float currHealth, float maxHealth)
    // {
    //     if (numTimesHalved < maxSplits && currHealth <= maxHealth * 0.5f)
    //     {
    //         numTimesHalved++;
    //         HalveAndDuplicate();
    //     }
    // }

    public LoopResult HandleGetHit(float damage, float remainingSlimeHealth)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            return new LoopResult(0, "Slime Boss defeated!", Color.red, transform.position);
        }
        return new LoopResult(0,  $"{Mathf.Ceil(remainingSlimeHealth)} more", Color.red, transform.position);
    }

    public void HandleSplit(Transform slimeTransform, float health, int splitsRemaining, float moveSpeed)
    {
        // Create two new slimes.
        for (int i=0; i < 2; i++)
        {
            GameObject newSlime = Instantiate(slimeChildPrefab, slimeTransform.position, transform.rotation);
            newSlime.transform.localScale = slimeTransform.localScale * 0.5f;
            // newSlime.transform.SetParent(transform);
            newSlime.GetComponent<RandomMovement>().moveSpeed = moveSpeed * 2f;
            SlimeChild slimeChild = newSlime.GetComponent<SlimeChild>();
            if (slimeChild != null)
            {
                slimeChild.SetSlimeBoss(this);
                slimeChild.SetMaxHealth(health / 2f);
                slimeChild.SetHealth(health / 2f);
                slimeChild.SetSplitsRemaining(splitsRemaining - 1);
                slimeChild.SetMoveSpeed(moveSpeed * 2f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
