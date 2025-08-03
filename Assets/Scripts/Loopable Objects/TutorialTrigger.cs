using UnityEngine;

public class TutorialTrigger : EnemyHealth
{
    [SerializeField]
    TutorialManager tutorialManager;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public override LoopResult HandleLooped(GameObject line, float multiplier)
    {
        Color spriteColor = GetComponentInChildren<SpriteRenderer>().color;
        currentHealth -= 1 * multiplier;
        if (currentHealth > 0)
        {
            return new LoopResult(0, $"{Mathf.Ceil(currentHealth)} more", spriteColor, transform.position);
        }
        Destroy(gameObject);
        tutorialManager.LoadSceneAfterDelay();
        return new LoopResult(0, "CORRUPTION ELIMINATED", spriteColor, transform.position);
    }
}
