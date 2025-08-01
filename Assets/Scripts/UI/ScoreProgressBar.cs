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
    [SerializeField] private Color backgroundColor = Color.gray;
    [SerializeField] private Gradient progressGradient;
    [SerializeField] private bool useGradient = true;

    [Header("Juice Effects")]
    [SerializeField] private float pulseIntensity = 1.1f;
    [SerializeField] private float pulseDuration = 0.2f;
    [SerializeField] private bool enableScoreChangeEffects = true;

    private float displayedScore = 0f;
    private float targetDisplayScore = 0f;
    private float velocity = 0f;
    private int lastKnownScore = 0;
    private Image fillImage;
    private Vector3 originalScale;

    void Start()
    {
        SetupProgressBar();
        originalScale = transform.localScale;
        if (scoreData != null)
        {
            lastKnownScore = scoreData.currentScore;
            displayedScore = lastKnownScore;
            targetDisplayScore = lastKnownScore;
        }
    }

    void Update()
    {
        if (scoreData == null) return;

        // Check for score changes
        if (scoreData.currentScore != lastKnownScore)
        {
            OnScoreChanged(scoreData.currentScore - lastKnownScore);
            lastKnownScore = scoreData.currentScore;
            targetDisplayScore = scoreData.currentScore;
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
        if (fillImage != null)
        {
            fillImage.color = fillColor;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Vertical;
        }
    }

    private void OnScoreChanged(int scoreChange)
    {
        if (enableScoreChangeEffects && scoreChange > 0)
        {
            StartCoroutine(PulseEffect());
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
        displayedScore = Mathf.Clamp(displayedScore, 0, scoreData.targetScore);
    }

    private void UpdateProgressBar()
    {
        if (progressBarFill == null || scoreData == null) return;

        float progress = scoreData.targetScore > 0 ? displayedScore / scoreData.targetScore : 0f;
        progress = Mathf.Clamp01(progress);
        Debug.Log(progress);

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

    public void SetColors(Color fill, Color background)
    {
        fillColor = fill;
        backgroundColor = background;
        if (fillImage != null && !useGradient)
        {
            fillImage.color = fillColor;
        }
    }
}
