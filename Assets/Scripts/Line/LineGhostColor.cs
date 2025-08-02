using System.Collections;
using UnityEngine;

public class LineGhostColor : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private LineGradient lineGradient;

    private Coroutine gradientCoroutine;
    private bool isGhostModeActive = false;
    private Gradient greyGradient;

    private Gradient originalGradient;
    private float ghostifyTimeLeft = 0f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineGradient = GetComponent<LineGradient>();
        // Create a grey gradient with 50% alpha
        greyGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = Color.grey;
        colorKeys[0].time = 0f;
        colorKeys[1].color = Color.grey;
        colorKeys[1].time = 1f;

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 0.5f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 0.5f;
        alphaKeys[1].time = 1f;

        greyGradient.SetKeys(colorKeys, alphaKeys);
    }

    private void Update()
    {
        if (isGhostModeActive && lineRenderer != null)
        {
            lineRenderer.colorGradient = greyGradient;
        }
    }

    public void GhostifyLine(float duration)
    {
        if (lineGradient != null)
        {
            if (gradientCoroutine != null)
            {
                StopCoroutine(gradientCoroutine);
            }
            gradientCoroutine = StartCoroutine(DisableGradientCoroutine(duration));
        }
    }

    private IEnumerator DisableGradientCoroutine(float duration)
    {
        lineGradient.enabled = false;
        isGhostModeActive = true;
        originalGradient = lineRenderer.colorGradient;
        ghostifyTimeLeft = duration;

        yield return new WaitForSeconds(duration);

        isGhostModeActive = false;
        lineGradient.enabled = true;
        if (lineRenderer != null && originalGradient != null)
        {
            lineRenderer.colorGradient = originalGradient;
        }
        gradientCoroutine = null;
        Destroy(this);

    }
}
