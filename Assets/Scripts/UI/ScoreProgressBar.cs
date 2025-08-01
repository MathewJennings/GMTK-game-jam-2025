using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScoreProgressBar : MonoBehaviour
{
    [SerializeField] private ScoreScriptableObject scoreData;

    [Header("UI Components")]
    [SerializeField] private RectTransform progressBarFill;
    [SerializeField] private TextMeshProUGUI currentScoreText;

    [Header("Animation Settings")]
    [SerializeField] private float animationSpeed = 2f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float momentumDecay = 0.95f;
    [SerializeField] private float smoothTime = 0.3f;

    [Header("Visual Effects")]
    [SerializeField] private Color fillColor = Color.green;
    [SerializeField] private Gradient progressGradient;
    [SerializeField] private bool useGradient = true;

    [Header("Juice Effects")]
    [SerializeField] private float pulseIntensity = 1.1f;
    [SerializeField] private float pulseDuration = 0.2f;
    [SerializeField] private bool enableScoreChangeEffects = true;

    [Header("Score Decay Settings")]
    [SerializeField] private bool enableScoreDecay = true;
    [SerializeField] private float minDecayRate = 0.5f; // Minimum decay when far from target score
    [SerializeField] private float maxDecayRate = 2.0f; // Maximum decay when close to target score
    [SerializeField] private AnimationCurve decayCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float decayGracePeriod = 2f; // Seconds of no decay after score increase
    [SerializeField] private float lowScoreThreshold = 0.2f; // Below this percentage, use gentler decay
    [SerializeField] private float highScoreThreshold = 0.8f; // Above this percentage, use aggressive decay

    private float displayedScore = 0f;
    private float targetDisplayScore = 0f;
    private float velocity = 0f;
    private float lastKnownScore = 0;
    private Image fillImage;
    private Vector3 originalScale;

    // Decay system variables
    private float lastScoreIncreaseTime = 0f;
    private float currentDecayRate = 0f;
    private bool isDecaying = false;
    private bool isDecayWarningEffectActive = false;

    void Start()
    {
        SetupProgressBar();
        originalScale = transform.localScale;
        if (scoreData != null)
        {
            lastKnownScore = scoreData.currentScore;
            displayedScore = lastKnownScore;
            targetDisplayScore = lastKnownScore;
            lastScoreIncreaseTime = Time.time;
        }
    }

    void Update()
    {
        if (scoreData == null ||
        (scoreData.hasLost && displayedScore <= 0) ||
        (scoreData.hasWon && displayedScore >= scoreData.targetScore)) return;

        // Check for score changes
        if (scoreData.currentScore != lastKnownScore)
        {
            float scoreChange = scoreData.currentScore - lastKnownScore;
            OnScoreChanged(scoreChange);
            lastKnownScore = scoreData.currentScore;
            targetDisplayScore = scoreData.currentScore;
            // Reset decay timer when score increases
            if (scoreChange > 0)
            {
                lastScoreIncreaseTime = Time.time;
            }
        }
        if (enableScoreDecay)
        {
            ApplyScoreDecay();
        }
        // Animate the displayed score with momentum
        AnimateScore();
        UpdateProgressBar();
        UpdateTextDisplay();
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

    private void ApplyScoreDecay()
    {
        // Check if we're in grace period
        float timeSinceLastIncrease = Time.time - lastScoreIncreaseTime;
        if (timeSinceLastIncrease < decayGracePeriod)
        {
            isDecaying = false;
            return;
        }
        if (scoreData.currentScore <= 0)
        {
            isDecaying = false;
            return;
        }
        float progressPercentage = scoreData.currentScore / scoreData.targetScore;
        float decayMultiplier = CalculateDecayMultiplier(progressPercentage);
        currentDecayRate = Mathf.Lerp(minDecayRate, maxDecayRate, decayMultiplier);
        float decayAmount = currentDecayRate * Time.deltaTime;
        float newScore = Mathf.Max(0, scoreData.currentScore - decayAmount);
        if (newScore != scoreData.currentScore)
        {
            scoreData.currentScore = newScore;
            isDecaying = true;
        }
    }

    private float CalculateDecayMultiplier(float progressPercentage)
        {
        if (progressPercentage <= lowScoreThreshold)
        {
            float lowScoreProgress = progressPercentage / lowScoreThreshold;
            return Mathf.Lerp(0f, 0.3f, decayCurve.Evaluate(lowScoreProgress));
        }
        else if (progressPercentage >= highScoreThreshold)
        {
            float highScoreProgress = (progressPercentage - highScoreThreshold) / (1f - highScoreThreshold);
            return Mathf.Lerp(0.7f, 1f, decayCurve.Evaluate(highScoreProgress));
        }
        else
        {
            float midProgress = (progressPercentage - lowScoreThreshold) / (highScoreThreshold - lowScoreThreshold);
            return Mathf.Lerp(0.3f, 0.7f, decayCurve.Evaluate(midProgress));
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
            else if (scoreChange < 0 && isDecaying && !isDecayWarningEffectActive)
            {
                StartCoroutine(DecayWarningEffect());
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

    private IEnumerator DecayWarningEffect()
    {
        isDecayWarningEffectActive = true;
        // Subtle red flash to indicate deca
        Color originalColor = fillImage.color;
        Color warningColor = Color.Lerp(originalColor, Color.red, 0.3f);

        float elapsedTime = 0f;
        float effectDuration = 0.5f;
        while (elapsedTime < effectDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / effectDuration;

            Color currentColor = progress < 0.5f
                ? Color.Lerp(originalColor, warningColor, progress * 2f)
                : Color.Lerp(warningColor, originalColor, (progress - 0.5f) * 2f);
            if (fillImage != null && !useGradient)
            {
                fillImage.color = currentColor;
            }
            yield return null;
        }
        if (fillImage != null && !useGradient)
        {
            fillImage.color = originalColor;
        }
        isDecayWarningEffectActive = false;
    }

    private void AnimateScore()
    {
        displayedScore = Mathf.SmoothDamp(displayedScore, targetDisplayScore, ref velocity, smoothTime);
        velocity *= momentumDecay;
        displayedScore = Mathf.Clamp(displayedScore, 0, scoreData.targetScore);
    }

    private void UpdateProgressBar()
    {
        if (progressBarFill == null || scoreData == null) return;

        float progress = scoreData.targetScore > 0 ? displayedScore / scoreData.targetScore : 0f;
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
        if (scoreData == null) return;

        if (currentScoreText != null)
        {
            currentScoreText.text = Mathf.RoundToInt(displayedScore).ToString();
        }
    }

    // Public methods for external control
    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = speed;
    }

    /// <summary>
    /// Temporarily pause score decay (useful for cutscenes, menus, powerups, etc.)
    /// </summary>
    public void PauseDecay(bool pause)
    {
        enableScoreDecay = !pause;
    }

    /// <summary>
    /// Reset the decay grace period (gives player more time before decay starts)
    /// </summary>
    public void ResetDecayGracePeriod()
    {
        lastScoreIncreaseTime = Time.time;
    }

    /// <summary>
    /// Get current decay rate for debugging/UI purposes
    /// </summary>
    public float GetCurrentDecayRate()
    {
        return currentDecayRate;
    }

    /// <summary>
    /// Check if score is currently decaying
    /// </summary>
    public bool IsDecaying()
    {
        return isDecaying && enableScoreDecay;
    }

    /// <summary>
    /// Get time remaining in grace period
    /// </summary>
    public float GetGracePeriodTimeRemaining()
    {
        float timeSinceLastIncrease = Time.time - lastScoreIncreaseTime;
        return Mathf.Max(0f, decayGracePeriod - timeSinceLastIncrease);
    }
}
