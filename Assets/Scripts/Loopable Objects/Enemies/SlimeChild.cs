using UnityEngine;

public class SlimeChild : MonoBehaviour, ILoopable
{
    private SlimeBoss slimeBoss;

    public float maxHealth;
    public float health;
    public int splitsRemaining = 0;
    public float moveSpeed = 1f;

    public void SetSlimeBoss(SlimeBoss boss)
    {
        slimeBoss = boss;
    }
    public void SetMaxHealth(float h)
    {
        maxHealth = h;
    }
    public void IncrementHealthAndMaxHealth(float h)
    {
        health += h;
        maxHealth += h;
    }
    public void SetHealth(float h)
    {
        health = h;
    }
    public void SetSplitsRemaining(int s)
    {
        splitsRemaining = s;
    }
    public void SetMoveSpeed(float s)
    {
        moveSpeed = s;
    }

    public LoopResult HandleLooped(GameObject line, float multiplier = 1f)
    {
        float damage = 1 * multiplier;
        if (damage > health)
        {
            damage = health;
        }
        health -= damage;
        Color spriteColor = GetComponentInChildren<SpriteRenderer>().color;
        LoopResult result = slimeBoss.HandleGetHit(damage, health, transform.position, spriteColor);

        if (ShouldSplit())
        {
            // Let the boss handle spawning new slimes based on stats of this one.
            slimeBoss.HandleSplit(transform, health, splitsRemaining, moveSpeed);
            Destroy(gameObject);
        }
        else if (health <= 0)
        {
            // If the slime is dead, just destroy it. We already passed on HandleGetHit
            Destroy(gameObject);
        }

        return result;
    }

    private bool ShouldSplit()
    {
        // Slime can still split, has less than half health, but still has some health left.
        return splitsRemaining > 0 && health <= maxHealth * 0.5f && health > 0;
    }
}
