using UnityEngine;

public class LineGradient : MonoBehaviour
{
    [SerializeField] private float cycleDuration = 3f;
    private LineRenderer lineRenderer;
    private LoopTextGenerator loopTextGenerator;

    private readonly Color hotPink = new Color(1f, 0f, 0.533f, 1f);    // ff0088
    private readonly Color cyan = new Color(0f, 0.8f, 1f, 1f);         // 00ccff
    private readonly Color yellow = new Color(1f, 0.867f, 0f, 1f);     // ffdd00

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
            currentColor = Color.Lerp(hotPink, cyan, localProgress);
        }
        else if (progress < 0.666f)
        {
            localProgress = (progress - 0.333f) / 0.333f;
            currentColor = Color.Lerp(cyan, yellow, localProgress);
        }
        else
        {
            localProgress = (progress - 0.666f) / 0.334f;
            currentColor = Color.Lerp(yellow, hotPink, localProgress);
        }
        GradientColorKey[] colorKeys = new GradientColorKey[4];
        colorKeys[0] = new GradientColorKey(currentColor, 0.0f);
        colorKeys[1] = new GradientColorKey(Color.Lerp(currentColor, hotPink, 0.3f), 0.33f);
        colorKeys[2] = new GradientColorKey(Color.Lerp(currentColor, cyan, 0.3f), 0.66f);
        Color colorAtEndOfLine = Color.Lerp(currentColor, yellow, 0.3f);
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
