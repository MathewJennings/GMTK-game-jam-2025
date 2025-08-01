using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopTextGenerator : MonoBehaviour
{
    [Header("UI Settings")]
    public float displayOffsetX = 100f; // Offset from the current mouse position
    public float displayOffsetY = 50f;
    public int fontSize = 24;
    public Color fontColor = Color.yellow;
    public float fadeDuration = 0.5f;
    public float verticalMovementAnimationDistance = 50f;

    public void SetFontColor(Color color)
    {
        fontColor = color;
    }

    public void CreateLoopCountText(string text, Vector2 currentCounterTextPosition, Color c)
    {
        CreateLoopCountText(text,currentCounterTextPosition, c, fontSize);
    }

    public void CreateLoopCountText(string text, Vector2 currentCounterTextPosition, bool useSmallFont)
    {
        int size = useSmallFont ? fontSize - 4 : fontSize;
        CreateLoopCountText(text,currentCounterTextPosition, fontColor, size);
    }

    public void CreateLoopCountText(string text, Vector2 currentCounterTextPosition, Color c, int fontSize)
    {
        GameObject loopCountTextGameObject = new($"Loop Count Text {text}");
        loopCountTextGameObject.transform.SetParent(transform, false);

        Text loopCountText = loopCountTextGameObject.AddComponent<Text>();
        loopCountText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        loopCountText.fontSize = fontSize;
        loopCountText.fontStyle = FontStyle.Bold;
        loopCountText.color = c;
        loopCountText.alignment = TextAnchor.MiddleCenter;
        loopCountText.text = text;

        Outline outline = loopCountTextGameObject.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, 2);

        RectTransform rectTransform = loopCountText.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 200);
        rectTransform.position = currentCounterTextPosition;

        StartCoroutine(AnimateTextFadeAndMovement(loopCountText, outline));
    }

    private IEnumerator AnimateTextFadeAndMovement(Text text, Outline outline)
    {
        Color originalTextColor = text.color;
        Color originalOutlineColor = outline.effectColor;
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        Vector3 startPosition = rectTransform.position;
        Vector3 endPosition = startPosition + new Vector3(0, verticalMovementAnimationDistance, 0);
        float totalMovementTime = fadeDuration * 3f; // fade in + stay + fade out

        // Start with transparent
        text.color = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, 0f);
        outline.effectColor = new Color(originalOutlineColor.r, originalOutlineColor.g, originalOutlineColor.b, 0f);

        float totalElapsedTime = 0f;
        while (totalElapsedTime < fadeDuration)
        {
            totalElapsedTime += Time.deltaTime;
            float alpha = totalElapsedTime / fadeDuration;
            text.color = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, alpha);
            outline.effectColor = new Color(originalOutlineColor.r, originalOutlineColor.g, originalOutlineColor.b, alpha);

            float movementProgress = totalElapsedTime / totalMovementTime;
            rectTransform.position = Vector3.Lerp(startPosition, endPosition, movementProgress);
            yield return null;
        }

        // Ensure fully visible
        text.color = originalTextColor;
        outline.effectColor = originalOutlineColor;

        float elapsedTimeBetweenFades = 0f;
        while (elapsedTimeBetweenFades < fadeDuration)
        {
            elapsedTimeBetweenFades += Time.deltaTime;
            totalElapsedTime += Time.deltaTime;
            float movementProgress = totalElapsedTime / totalMovementTime;
            rectTransform.position = Vector3.Lerp(startPosition, endPosition, movementProgress);
            yield return null;
        }

        // Fade out
        float fadeOutElapsedTime = 0f;
        while (fadeOutElapsedTime < fadeDuration)
        {
            fadeOutElapsedTime += Time.deltaTime;
            totalElapsedTime += Time.deltaTime;
            float alpha = 1f - (fadeOutElapsedTime / fadeDuration);
            text.color = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, alpha);
            outline.effectColor = new Color(originalOutlineColor.r, originalOutlineColor.g, originalOutlineColor.b, alpha);

            float movementProgress = totalElapsedTime / totalMovementTime;
            rectTransform.position = Vector3.Lerp(startPosition, endPosition, movementProgress);
            yield return null;
        }
        Destroy(text.gameObject);
    }
}
