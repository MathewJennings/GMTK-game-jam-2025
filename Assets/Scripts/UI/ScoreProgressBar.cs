using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScoreProgressBar : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

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

    [Header("Score Decay Settings")]
    [SerializeField] private bool isScoreDecayEnabled = true;
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

    void Awake()
    {
        SetupProgressBar();
        originalScale = transform.localScale;
        if (levelManager == null)
        {
            Debug.LogWarning("ScoreProgressBar: Missing reference to LevelManager.");
        }
        else
        {
            lastKnownScore = levelManager.currentLevel.currentPoints;
            displayedScore = lastKnownScore;
            targetDisplayScore = lastKnownScore;
            lastScoreIncreaseTime = Time.time;
        }
    }

    void Update()
    {
        UpdateScoreDisplayState();
        MaybeApplyScoreDecay();
        AnimateScore();
        UpdateProgressBar();
        UpdateTextDisplay();
    }

    private void UpdateScoreDisplayState()
    {
        if (levelManager.currentLevel.currentPoints != lastKnownScore)
        {
            float scoreChange = levelManager.currentLevel.currentPoints - lastKnownScore;
            OnScoreChanged(scoreChange);
            lastKnownScore = levelManager.currentLevel.currentPoints;
            targetDisplayScore = levelManager.currentLevel.currentPoints;
            // Reset decay timer when score increases
            if (scoreChange > 0)
            {
                lastScoreIncreaseTime = Time.time;
            }
        }
    }

    private void MaybeApplyScoreDecay()
    {
        if (isScoreDecayEnabled &&
                !(levelManager.currentLevel.HasRunOutOfPoints() && displayedScore <= 0) &&
                !levelManager.currentLevel.isBossFight)
        {
            ApplyScoreDecay();
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

    private void ApplyScoreDecay()
    {
        // Check if we're in grace period
        float timeSinceLastIncrease = Time.time - lastScoreIncreaseTime;
        if (timeSinceLastIncrease < decayGracePeriod)
        {
            isDecaying = false;
            return;
        }
        if (levelManager.currentLevel.currentPoints <= 0)
        {
            isDecaying = false;
            return;
        }
        float progressPercentage = levelManager.currentLevel.currentPoints / levelManager.currentLevel.targetPoints;
        float decayMultiplier = CalculateDecayMultiplier(progressPercentage);
        currentDecayRate = Mathf.Lerp(minDecayRate, maxDecayRate, decayMultiplier);
        float decayAmount = currentDecayRate * Time.deltaTime;
        float newScore = Mathf.Max(0, levelManager.currentLevel.currentPoints - decayAmount);
        if (newScore != levelManager.currentLevel.currentPoints)
        {
            levelManager.currentLevel.currentPoints = newScore;
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
        displayedScore = Mathf.Clamp(displayedScore, 0, levelManager.currentLevel.targetPoints);
    }

    private void UpdateProgressBar()
    {
        if (progressBarFill == null || levelManager == null) return;

        float progress = levelManager.currentLevel.targetPoints > 0 ? displayedScore / levelManager.currentLevel.targetPoints : 0f;
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

    /// <summary>
    /// Temporarily pause score decay (useful for cutscenes, menus, powerups, etc.)
    /// </summary>
    public void PauseDecay(bool pause)
    {
        isScoreDecayEnabled = !pause;
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
        return isDecaying && isScoreDecayEnabled;
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
