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

        GradientColorKey[] colorKeys = new GradientColorKey[4];
        colorKeys[0] = new GradientColorKey(Color.HSVToRGB(progressPercentage * 0.1f % 1f, 0.8f, 1f), 0.0f);
        colorKeys[1] = new GradientColorKey(Color.HSVToRGB((progressPercentage * 0.1f + 0.2f) % 1f, 0.8f, 1f), 0.33f);
        colorKeys[2] = new GradientColorKey(Color.HSVToRGB((progressPercentage * 0.1f + 0.4f) % 1f, 0.8f, 1f), 0.66f);
        colorKeys[3] = new GradientColorKey(Color.HSVToRGB((progressPercentage * 0.1f + 0.6f) % 1f, 0.8f, 1f), 1.0f);

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);

        currentGradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = currentGradient;
    }
}
