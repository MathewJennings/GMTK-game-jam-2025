using UnityEngine;

public class LineGradient : MonoBehaviour
{
    [SerializeField]
    private Gradient lineColorGradient;

    [SerializeField]
    private int maxLineLength;

    private LineRenderer lineRenderer;
    private LoopTextGenerator _loopTextGenerator;

    public void SetMaxLineLength(int length)
    {
        maxLineLength = length;
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Canvas canvas = FindFirstObjectByType<Canvas>();
        _loopTextGenerator = canvas.GetComponent<LoopTextGenerator>();
        UpdateGradient();
    }

    public void UpdateGradient()
    {
        Gradient currentGradient = new();
        // Create a rich gradient of green hues from vibrant lime to dark forest green
        float timeShift = (Mathf.Sin(Time.time * 0.8f) + 1f) * 0.5f; // Gentle sine wave oscillation (0 to 1)

        GradientColorKey[] colorKeys = new GradientColorKey[8];
        // Green hue range: 0.25 (lime) to 0.4 (forest), with varying saturation and brightness
        float greenHueBase = 0.33f; // Base green hue
        float hueVariation = 0.1f;  // Range of green hues to cover
        colorKeys[0] = new GradientColorKey(Color.HSVToRGB(greenHueBase - hueVariation + timeShift * 0.1f, 0.6f, 1f), 0.0f);   // Bright lime green
        colorKeys[1] = new GradientColorKey(Color.HSVToRGB(greenHueBase - 0.05f + timeShift * 0.1f, 0.7f, 0.95f), 0.143f);     // Light green
        colorKeys[2] = new GradientColorKey(Color.HSVToRGB(greenHueBase + timeShift * 0.1f, 0.8f, 0.9f), 0.286f);              // Medium green
        colorKeys[3] = new GradientColorKey(Color.HSVToRGB(greenHueBase + 0.02f + timeShift * 0.1f, 0.85f, 0.8f), 0.429f);     // Darker green
        colorKeys[4] = new GradientColorKey(Color.HSVToRGB(greenHueBase + 0.04f + timeShift * 0.1f, 0.9f, 0.7f), 0.571f);      // Forest green
        colorKeys[5] = new GradientColorKey(Color.HSVToRGB(greenHueBase + 0.06f + timeShift * 0.1f, 0.95f, 0.6f), 0.714f);     // Deep green
        colorKeys[6] = new GradientColorKey(Color.HSVToRGB(greenHueBase + 0.08f + timeShift * 0.1f, 1f, 0.5f), 0.857f);        // Dark forest green
        Color colorAtEndOfLine = Color.HSVToRGB(greenHueBase + hueVariation + timeShift * 0.1f, 1f, 0.4f);                     // Very dark green
        colorKeys[7] = new GradientColorKey(colorAtEndOfLine, 1.0f);

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);

        currentGradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = currentGradient;
        _loopTextGenerator.SetFontColor(colorAtEndOfLine);
    }
}
