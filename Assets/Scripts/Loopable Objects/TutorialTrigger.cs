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
        currentHealth -= 1 * multiplier;
        if (currentHealth > 0)
        {
            return new LoopResult(0, $"{Mathf.Ceil(currentHealth)} more", ColorPalette.BrightYellow, transform.position);
        }
        Destroy(gameObject);
        tutorialManager.LoadSceneAfterDelay();
        Color spriteColor = GetComponentInChildren<SpriteRenderer>().color;
        return new LoopResult(0, "CORRUPTION ELIMINATED", spriteColor, transform.position);
    }
}
