using UnityEngine;

public class LineGradient : MonoBehaviour
{
    [SerializeField]
    private Gradient lineColorGradient;

    [SerializeField]
    private int maxLineLength;

    private LineRenderer lineRenderer;

    public void SetMaxLineLength(int length)
    {
        maxLineLength = length;
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        InitializeGradient();
    }

    private void InitializeGradient()
    {
        if (lineColorGradient == null || lineColorGradient.colorKeys.Length == 0)
        {
            lineColorGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.cyan, 0.0f);
            colorKeys[1] = new GradientColorKey(Color.magenta, 0.5f);
            colorKeys[2] = new GradientColorKey(Color.yellow, 1.0f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
            alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);

            lineColorGradient.SetKeys(colorKeys, alphaKeys);
        }
        lineRenderer.colorGradient = lineColorGradient;
    }

    public void UpdateGradient(int currentPointCount)
    {
        Gradient currentGradient = new();
        float progressPercentage = (float)currentPointCount / maxLineLength;

        // Create a rainbow effect that cycles through all colors (hues) as you draw
        GradientColorKey[] colorKeys = new GradientColorKey[8];
        // Adding 0.33f offset to make the base hue green
        float baseHue = (progressPercentage * 0.15f + 0.33f) % 1f;
        float saturation = .8f;
        colorKeys[0] = new GradientColorKey(Color.HSVToRGB(baseHue % 1f, saturation, 1f), 0.0f);
        colorKeys[1] = new GradientColorKey(Color.HSVToRGB((baseHue + 0.125f) % 1f, saturation, 1f), 0.143f);
        colorKeys[2] = new GradientColorKey(Color.HSVToRGB((baseHue + 0.25f) % 1f, saturation, 1f), 0.286f);
        colorKeys[3] = new GradientColorKey(Color.HSVToRGB((baseHue + 0.375f) % 1f, saturation, 1f), 0.429f);
        colorKeys[4] = new GradientColorKey(Color.HSVToRGB((baseHue + 0.5f) % 1f, saturation, 1f), 0.571f);
        colorKeys[5] = new GradientColorKey(Color.HSVToRGB((baseHue + 0.625f) % 1f, saturation, 1f), 0.714f);
        colorKeys[6] = new GradientColorKey(Color.HSVToRGB((baseHue + 0.75f) % 1f, saturation, 1f), 0.857f);
        colorKeys[7] = new GradientColorKey(Color.HSVToRGB((baseHue + 0.875f) % 1f, saturation, 1f), 1.0f);

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);

        currentGradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = currentGradient;
    }
}
