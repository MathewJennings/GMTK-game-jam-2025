using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WaveProgressBar : MonoBehaviour, IBossObserver
{
    [SerializeField] private LevelManager levelManager;

    [Header("UI Components")]
    [SerializeField] private RectTransform progressBarFill;
    [SerializeField] private TextMeshProUGUI currentScoreText;

    [Header("Animation Settings")]
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float momentumDecay = 0.95f;
    [SerializeField] private float smoothTime = 0.3f;

    [Header("Juice Effects")]
    [SerializeField] private float pulseIntensity = 1.1f;
    [SerializeField] private float pulseDuration = 0.2f;
    [SerializeField] private bool enableScoreChangeEffects = true;

    [Header("Score Corruption Settings")]
    [SerializeField] private bool isAddCorruptionEnabled = true;
    [SerializeField] private float minCorruptRate = 0.5f; // Minimum corruption when close to max corruption
    [SerializeField] private float maxCorruptRate = 2.0f; // Maximum corruption when close to zero corruption
    [SerializeField] private AnimationCurve corruptionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float corruptionGracePeriod = 2f; // Seconds of no corruption after corruption decrease
    [SerializeField] private float lowCorruptionThreshold = 0.2f; // Below this percentage, use aggressive corruption
    [SerializeField] private float highCorruptionThreshold = 0.8f; // Above this percentage, use gentle corruption

    private float displayedCorruption = 0f;
    private float targetDisplayCorruption = 0f;
    private float velocity = 0f;
    private float lastKnownCorruption = 0;
    private Image fillImage;
    private Vector3 originalScale;
    private Color originalColor;

    // Corruption system variables
    private float lastCorruptionDecreaseTime = 0f;
    private float currentCorrutionRate = 0f;
    private bool isCorrupting = false;
    private bool isCorruptionWarningEffectActive = false;
    private Coroutine corruptionWarningCoroutine;
    private bool corruptionPaused = false;
    private Coroutine pauseCorruptionCoroutine;

    void Awake()
    {
        SetupProgressBar();
        originalScale = transform.localScale;
        originalColor = fillImage.color;
        if (levelManager == null)
        {
            Debug.LogWarning("WaveProgressBar: Missing reference to LevelManager.");
        }
        else
        {
            lastKnownCorruption = levelManager.currentLevel.currentCorruption;
            displayedCorruption = lastKnownCorruption;
            targetDisplayCorruption = lastKnownCorruption;
            lastCorruptionDecreaseTime = Time.time;
        }
        levelManager.RegisterBossObserver(this);
    }

    void OnDestroy()
    {
        if (levelManager != null)
        {
            levelManager.UnregisterBossObserver(this);
        }
    }

    void Update()
    {
        UpdateCorruptionDisplayState();
        MaybeApplyAdditionalCorruption();
        AnimateCorruption();
        UpdateProgressBar();
        UpdateTextDisplay();
    }

    private void UpdateCorruptionDisplayState()
    {
        if (levelManager.currentLevel.currentCorruption != lastKnownCorruption)
        {
            float corruptionChange = levelManager.currentLevel.currentCorruption - lastKnownCorruption;
            OnCorruptionChanged(corruptionChange);
            lastKnownCorruption = levelManager.currentLevel.currentCorruption;
            targetDisplayCorruption = levelManager.currentLevel.currentCorruption;
            // Reset corruption timer when corruption decreases
            if (corruptionChange < 0)
            {
                lastCorruptionDecreaseTime = Time.time;
            }
        }
    }

    private void MaybeApplyAdditionalCorruption()
    {
        if (isAddCorruptionEnabled &&
                !(levelManager.currentLevel.HasReachedMaxCorruption() && displayedCorruption < levelManager.currentLevel.totalCorruption))
        {
            ApplyCorruption();
        }
    }

    public void NotifyBossSpawned()
    {
        isAddCorruptionEnabled = false;
    }

    public void NotifyBossDefeated()
    {
        isAddCorruptionEnabled = true;
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

    private void ApplyCorruption()
    {
        // Check if we're in grace period
        float timeSinceLastDecrease = Time.time - lastCorruptionDecreaseTime;
        if (timeSinceLastDecrease < corruptionGracePeriod)
        {
            isCorrupting = false;
            return;
        }
        if (levelManager.currentLevel.HasReachedMaxCorruption())
        {
            isCorrupting = false;
            return;
        }
        if (corruptionPaused)
        {
            isCorrupting = false;
            return;
        }
        float progressPercentage = levelManager.currentLevel.currentCorruption / levelManager.currentLevel.totalCorruption;
        float corruptionMultiplier = CalculateCorruptionMultiplier(progressPercentage);
        currentCorrutionRate = Mathf.Lerp(minCorruptRate, maxCorruptRate, corruptionMultiplier);
        float corruptAmount = currentCorrutionRate * Time.deltaTime;
        float newCorruption = Mathf.Max(0, levelManager.currentLevel.currentCorruption + corruptAmount);
        if (newCorruption != levelManager.currentLevel.currentCorruption)
        {
            levelManager.currentLevel.currentCorruption = newCorruption;
            isCorrupting = true;
        }
    }

    private float CalculateCorruptionMultiplier(float progressPercentage)
        {
        if (progressPercentage <= lowCorruptionThreshold)
        {
            float lowCorruptionProgress = progressPercentage / lowCorruptionThreshold;
            return Mathf.Lerp(0.7f, 1f, corruptionCurve.Evaluate(lowCorruptionProgress));
        }
        else if (progressPercentage >= highCorruptionThreshold)
        {
            float highCorruptionProgress = (progressPercentage - highCorruptionThreshold) / (1f - highCorruptionThreshold);
            return Mathf.Lerp(0f, 0.3f, corruptionCurve.Evaluate(highCorruptionProgress));
        }
        else
        {
            float midProgress = (progressPercentage - lowCorruptionThreshold) / (highCorruptionThreshold - lowCorruptionThreshold);
            return Mathf.Lerp(0.3f, 0.7f, corruptionCurve.Evaluate(midProgress));
        }
    }

    private void OnCorruptionChanged(float corruptionChange)
    {
        if (enableScoreChangeEffects)
        {
            if (corruptionChange < 0)
            {
                StartCoroutine(PulseEffect());
            }
            else if (corruptionChange > 0 && isCorrupting && !corruptionPaused && !isCorruptionWarningEffectActive)
            {
                corruptionWarningCoroutine = StartCoroutine(CorruptionWarningEffect());
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

    private IEnumerator CorruptionWarningEffect()
    {
        isCorruptionWarningEffectActive = true;
        // Subtle black flash to indicate corruption
        Color warningColor = Color.Lerp(originalColor, Color.black, 0.3f);

        float elapsedTime = 0f;
        float effectDuration = 0.5f;
        while (elapsedTime < effectDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / effectDuration;

            Color currentColor = progress < 0.5f
                ? Color.Lerp(originalColor, warningColor, progress * 2f)
                : Color.Lerp(warningColor, originalColor, (progress - 0.5f) * 2f);
            if (fillImage != null)
            {
                fillImage.color = currentColor;
                Debug.Log(fillImage.color);
            }
            yield return null;
        }
        if (fillImage != null)
        {
            fillImage.color = originalColor;
        }
        isCorruptionWarningEffectActive = false;
    }

    private void AnimateCorruption()
    {
        displayedCorruption = Mathf.SmoothDamp(displayedCorruption, targetDisplayCorruption, ref velocity, smoothTime);
        velocity *= momentumDecay;
        displayedCorruption = Mathf.Clamp(displayedCorruption, 0, levelManager.currentLevel.totalCorruption);
    }

    private void UpdateProgressBar()
    {
        if (progressBarFill == null || levelManager == null) return;

        float progress = levelManager.currentLevel.totalCorruption > 0 ? displayedCorruption / levelManager.currentLevel.totalCorruption : 0f;
        progress = Mathf.Clamp01(progress);
        if (fillImage != null)
        {
            fillImage.fillAmount = progress;
        }
    }

    private void UpdateTextDisplay()
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = Mathf.RoundToInt(displayedCorruption).ToString();
        }
    }

    /// <summary>
    /// Temporarily pause corruption (useful for cutscenes, menus, powerups, etc.)
    /// </summary>
    public void PauseCorruption(float duration)
    {
        // Stop current corruption coroutine so we can reset the duration.
        if (pauseCorruptionCoroutine != null)
        {
            StopCoroutine(pauseCorruptionCoroutine);
        }
        // Stop corruption warning coroutine to avoid interfering with color changes.
        if (corruptionWarningCoroutine != null)
        {
            StopCoroutine(corruptionWarningCoroutine);
            corruptionWarningCoroutine = null;
            isCorruptionWarningEffectActive = false;
        }

        pauseCorruptionCoroutine = StartCoroutine(PauseCorruptionCoroutine(duration));
    }

    private IEnumerator PauseCorruptionCoroutine(float duration)
    {
        isAddCorruptionEnabled = false;
        corruptionPaused = true;
        // Lerp to a light blue ice color
        Color corruptionFrozenColor = new Color(100 / 255f, 255 / 255f, 255 / 255f); // Normalize RGB values to 0-1
        float transitionDuration = 0.3f;

        for (float t = 0; t < transitionDuration; t += Time.deltaTime)
        {
            fillImage.color = Color.Lerp(originalColor, corruptionFrozenColor, t / transitionDuration);
            yield return null;
        }
        fillImage.color = corruptionFrozenColor;

        yield return new WaitForSeconds(duration);

        // Lerp back to the original color over 0.3 seconds
        for (float t = 0; t < transitionDuration; t += Time.deltaTime)
        {
            fillImage.color = Color.Lerp(corruptionFrozenColor, originalColor, t / transitionDuration);
            yield return null;
        }
        fillImage.color = originalColor;
        isAddCorruptionEnabled = true;
        corruptionPaused = false;
        pauseCorruptionCoroutine = null;
    }

    /// <summary>
    /// Reset the corruption grace period (gives player more time before corruption starts)
    /// </summary>
    public void ResetCorruptionGracePeriod()
    {
        lastCorruptionDecreaseTime = Time.time;
    }

    public float GetCurrentCorruptionRate()
    {
        return currentCorrutionRate;
    }

    public bool IsCorrupting()
    {
        return isCorrupting && isAddCorruptionEnabled;
    }

    public float GetGracePeriodTimeRemaining()
    {
        float timeSinceLastDecrease = Time.time - lastCorruptionDecreaseTime;
        return Mathf.Max(0f, corruptionGracePeriod - timeSinceLastDecrease);
    }
}
