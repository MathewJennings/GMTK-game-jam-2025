using UnityEngine;

public class LineGradient : MonoBehaviour
{
    [SerializeField] private float cycleDuration = 3f;
    private LineRenderer lineRenderer;
    private LoopTextGenerator loopTextGenerator;

    public void SetLoopTextGenerator(LoopTextGenerator loopTextGenerator)
    {
        this.loopTextGenerator = loopTextGenerator;
        UpdateGradient();
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void UpdateGradient()
    {
        Gradient currentGradient = new();
        float progress = Time.time % cycleDuration / cycleDuration;
        Color currentColor;
        float localProgress;
        if (progress < 0.333f)
        {
            localProgress = progress / 0.333f;
            currentColor = Color.Lerp(ColorPalette.HotPink, ColorPalette.ElectricCyan, localProgress);
        }
        else if (progress < 0.666f)
        {
            localProgress = (progress - 0.333f) / 0.333f;
            currentColor = Color.Lerp(ColorPalette.ElectricCyan, ColorPalette.BrightYellow, localProgress);
        }
        else
        {
            localProgress = (progress - 0.666f) / 0.334f;
            currentColor = Color.Lerp(ColorPalette.BrightYellow, ColorPalette.HotPink, localProgress);
        }
        GradientColorKey[] colorKeys = new GradientColorKey[4];
        colorKeys[0] = new GradientColorKey(currentColor, 0.0f);
        colorKeys[1] = new GradientColorKey(Color.Lerp(currentColor, ColorPalette.HotPink, 0.3f), 0.33f);
        colorKeys[2] = new GradientColorKey(Color.Lerp(currentColor, ColorPalette.ElectricCyan, 0.3f), 0.66f);
        Color colorAtEndOfLine = Color.Lerp(currentColor, ColorPalette.BrightYellow, 0.3f);
        colorKeys[3] = new GradientColorKey(colorAtEndOfLine, 1.0f);

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
        alphaKeys[0] = new GradientAlphaKey(0.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(0.5f, 0.333f);
        alphaKeys[2] = new GradientAlphaKey(1.0f, 1.0f);

        currentGradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = currentGradient;
        loopTextGenerator.SetFontColor(colorAtEndOfLine);
    }
}
