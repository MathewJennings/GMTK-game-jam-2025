using System.Collections.Generic;
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

    [SerializeField]
    private bool canGrow = true;

    private readonly List<SlimeChild> slimeChildren = new();

    public override void AddHealth(float additonalHealth)
    {
        float oldHealth = currentHealth;
        // currentHealth = Mathf.Clamp(currentHealth + additonalHealth, 0, maxHealth);
        base.AddHealth(additonalHealth);
        float addedHealth = currentHealth - oldHealth;
        slimeChildren.RemoveAll(child => child == null);
        foreach (SlimeChild child in slimeChildren)
        {
            child.IncrementHealthAndMaxHealth(addedHealth/slimeChildren.Count);
        }
    }

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
        slime.GetComponent<RandomMovement>().moveSpeed = initialMoveSpeed;
        SlimeChild slimeChild = slime.GetComponent<SlimeChild>();
        slimeChildren.Add(slimeChild);
        if (slimeChild != null)
        {
            slimeChild.SetSlimeBoss(this);
            slimeChild.canGrow = canGrow;
            slimeChild.SetOriginalMaxHealth(maxHealth);
            slimeChild.SetMaxHealth(maxHealth);
            slimeChild.SetHealth(maxHealth);
            slimeChild.SetSplitsRemaining(maxSplits);
            slimeChild.SetMoveSpeed(initialMoveSpeed);
            slimeChild.GetComponent<RandomMovement>().moveSpeed = initialMoveSpeed;
        }
    }

    public LoopResult HandleGetHit(float damage, float remainingSlimeHealth, Vector3 slimePosition)
    {
        currentHealth -= damage;
        // There might be floating point errors with reporting health from all the children, so checking health against
        // a small threshold is sufficient.
        if (currentHealth <= 0.001)
        {
            return OnDefeatBoss();
        }
        float v = Mathf.Ceil(remainingSlimeHealth);
        string text = v > 0 ? $"{v} more" : "DEFEATED!";
        return new LoopResult(0,  text, Color.red, slimePosition);
    }

    public void HandleSplit(Transform slimeTransform, float health, int splitsRemaining, float moveSpeed)
    {
        // Create two new slimes.
        for (int i=0; i < 2; i++)
        {
            GameObject newSlime = Instantiate(slimeChildPrefab, slimeTransform.position, transform.rotation);
            newSlime.transform.localScale = slimeTransform.localScale * 0.5f;
            newSlime.GetComponent<RandomMovement>().moveSpeed = moveSpeed * 1.5f;
            SlimeChild slimeChild = newSlime.GetComponent<SlimeChild>();
            slimeChildren.Add(slimeChild);
            if (slimeChild != null)
            {
                slimeChild.SetSlimeBoss(this);
                slimeChild.canGrow = canGrow;
                slimeChild.SetOriginalMaxHealth(health / 2f);
                slimeChild.SetMaxHealth(health / 2f);
                slimeChild.SetHealth(health / 2f);
                slimeChild.SetSplitsRemaining(splitsRemaining - 1);
                slimeChild.SetMoveSpeed(moveSpeed * 1.5f);
            }
        }
    }
}
