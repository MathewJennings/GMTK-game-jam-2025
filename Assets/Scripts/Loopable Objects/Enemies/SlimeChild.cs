using System.Collections;
using UnityEngine;

public class SlimeChild : MonoBehaviour, ILoopable
{
    [SerializeField]
    public bool canGrow = true;
    
    private SlimeBoss slimeBoss;

    public float originalMaxHealth;
    public float maxHealth;
    public float health;
    public int splitsRemaining = 0;
    public float moveSpeed = 1f;

    void Update()
    {
        if (canGrow && ShouldGrow())
        {
            // TODO: lerp the scale
            StartCoroutine(LerpScale(transform.localScale, transform.localScale * 2, 1f)); // Lerp over 1 second
            // transform.localScale *= 2;
            moveSpeed *= 0.66f;
            GetComponent<RandomMovement>().moveSpeed = moveSpeed;
            originalMaxHealth = maxHealth;
            splitsRemaining++;
        }
    }

    private IEnumerator LerpScale(Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale; // Ensure the final scale is set
    }

    public void SetSlimeBoss(SlimeBoss boss)
    {
        slimeBoss = boss;
    }

    public void SetOriginalMaxHealth(float h)
    {
        originalMaxHealth = h;
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
        LoopResult result = slimeBoss.HandleGetHit(damage, health, transform.position);

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

    private bool ShouldGrow()
    {
        return maxHealth > 4 * originalMaxHealth;
    }
}
