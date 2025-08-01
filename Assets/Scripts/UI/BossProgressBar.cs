using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossProgressBar : MonoBehaviour
{
    private BossHealth bossHealth;

    [Header("UI Components")]
    [SerializeField] private RectTransform progressBarFill;
    [SerializeField] private TextMeshProUGUI currentScoreText;

    [Header("Animation Settings")]
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float momentumDecay = 0.95f;
    [SerializeField] private float smoothTime = 0.3f;

    [Header("Visual Effects")]
    [SerializeField] private Gradient progressGradient;
    [SerializeField] private bool useGradient = true;

    [Header("Juice Effects")]
    [SerializeField] private float pulseIntensity = 1.1f;
    [SerializeField] private float pulseDuration = 0.2f;
    [SerializeField] private bool enableScoreChangeEffects = true;

    private float displayedScore = 0f;
    private float targetDisplayScore = 0f;
    private float velocity = 0f;
    private float lastKnownScore = 0;
    private Image fillImage;
    private Vector3 originalScale;

    public void SetBossHealth(BossHealth bossHealth)
    {
        this.bossHealth = bossHealth;
    }

    void Awake()
    {
        SetupProgressBar();
        originalScale = transform.localScale;
        if (bossHealth == null)
        {
            Debug.LogWarning("BossProgressBar: Missing reference to BossHealth.");
        }
        else
        {
            lastKnownScore = bossHealth.GetCurrentHealth();
            displayedScore = lastKnownScore;
            targetDisplayScore = lastKnownScore;
        }
    }

    void Update()
    {
        UpdateScoreDisplayState();
        AnimateScore();
        UpdateProgressBar();
        UpdateTextDisplay();
    }

    private void UpdateScoreDisplayState()
    {
        if (bossHealth.GetCurrentHealth() != lastKnownScore)
        {
            float scoreChange = bossHealth.GetCurrentHealth() - lastKnownScore;
            OnScoreChanged(scoreChange);
            lastKnownScore = bossHealth.GetCurrentHealth();
            targetDisplayScore = bossHealth.GetCurrentHealth();
        }
    }

    private void SetupProgressBar()
    {
        if (fillImage == null && progressBarFill != null)
        {
            fillImage = progressBarFill.GetComponent<Image>();
            if (fillImage == null)
            {
                fillImage = progressBarFill.gameObject.AddComponent<Image>();
            }
        }
    }

    private void OnScoreChanged(float scoreChange)
    {
        if (enableScoreChangeEffects)
        {
            if (scoreChange > 0)
            {
                StartCoroutine(PulseEffect());
            }
        }
    }

    private IEnumerator PulseEffect()
    {
        float elapsedTime = 0f;
        while (elapsedTime < pulseDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / pulseDuration;
            float scaleMultiplier = Mathf.Lerp(pulseIntensity, 1f, animationCurve.Evaluate(progress));
            transform.localScale = originalScale * scaleMultiplier;
            yield return null;
        }
        transform.localScale = originalScale;
    }

    private void AnimateScore()
    {
        displayedScore = Mathf.SmoothDamp(displayedScore, targetDisplayScore, ref velocity, smoothTime);
        velocity *= momentumDecay;
        displayedScore = Mathf.Clamp(displayedScore, 0, bossHealth.GetMaxHealth());
    }

    private void UpdateProgressBar()
    {
        if (progressBarFill == null) return;
        float progress = 0f;
        if (bossHealth != null)
        {
            progress = bossHealth.GetMaxHealth() > 0 ? displayedScore / bossHealth.GetMaxHealth() : 0f;
        }
        progress = Mathf.Clamp01(progress);

        // Update fill amount
        if (fillImage != null)
        {
            fillImage.fillAmount = progress;
            if (useGradient && progressGradient != null)
            {
                fillImage.color = progressGradient.Evaluate(progress);
            }
        }
    }

    private void UpdateTextDisplay()
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = Mathf.RoundToInt(displayedScore).ToString();
        }
    }
}
