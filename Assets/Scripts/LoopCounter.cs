using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopCounter : MonoBehaviour
{
    [Header("UI Settings")]
    public float displayOffsetX = 100f; // Offset from the current mouse position
    public float displayOffsetY = 50f;
    public int fontSize = 24;
    public Color fontColor = Color.yellow;
    public float fadeDuration = 0.5f;
    public float verticalMovementAnimationDistance = 50f;

    private int currentLoopCount = 0;
    private Camera mainCamera;
    private Canvas canvas;
    private List<GameObject> loopCountTextGameObjects;
    private List<Text> loopCountTexts;
    private Vector2 currentCounterTextPosition;

    void Start()
    {
        mainCamera = Camera.main;
        canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            CreateCanvas();
        }
        loopCountTextGameObjects = new List<GameObject>();
        loopCountTexts = new List<Text>();
    }

    /// <summary>
    /// Set the display offset for loop counter text positioning.
    /// </summary>
    /// <param name="offsetX">Horizontal offset from drawing position</param>
    /// <param name="offsetY">Vertical offset from drawing position</param>
    public void SetDisplayOffset(float offsetX, float offsetY)
    {
        displayOffsetX = offsetX;
        displayOffsetY = offsetY;
    }

    public void SetFontSize(int size)
    {
        fontSize = size < 8 ? 8 : size;
    }

    public void SetFontColor(Color color)
    {
        fontColor = color;
    }

    public void SetFadeDuration(float duration)
    {
        fadeDuration = duration < 0.1f ? 0.1f : duration;
    }

    public void SetVerticalMovementAnimationDistance(float distance)
    {
        verticalMovementAnimationDistance = distance;
    }

    private void CreateCanvas()
    {
        GameObject canvasObj = new("Loop Counter Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // High sorting order to appear on top
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();
    }

    /// <summary>
    /// Increment the loop count and create a new loop count text to display it.
    /// </summary>
    public void IncrementLoopCount()
    {
        currentLoopCount++;
        CreateLoopCountText(currentLoopCount);
    }

    private void CreateLoopCountText(int loopNumber)
    {
        GameObject loopCountTextGameObject = new($"Loop Count Text {loopNumber}");
        loopCountTextGameObject.transform.SetParent(canvas.transform, false);

        Text loopCountText = loopCountTextGameObject.AddComponent<Text>();
        loopCountText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        loopCountText.fontSize = fontSize;
        loopCountText.fontStyle = FontStyle.Bold;
        loopCountText.color = fontColor;
        loopCountText.alignment = TextAnchor.MiddleCenter;
        loopCountText.text = loopNumber.ToString();

        Outline outline = loopCountTextGameObject.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, 2);

        RectTransform rectTransform = loopCountText.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 100);
        rectTransform.position = currentCounterTextPosition;

        loopCountTextGameObjects.Add(loopCountTextGameObject);
        loopCountTexts.Add(loopCountText);

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
    }

    /// <summary>
    /// Update the position where we will display the counter for the next loop.
    /// </summary>
    /// <param name="worldPosition">The current world position of the line drawing.</param>
    public void UpdateCounterTextPosition(Vector2 worldPosition)
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
        screenPosition.x += displayOffsetX;
        screenPosition.y += displayOffsetY;
        currentCounterTextPosition = screenPosition;
    }

    public void DestroyLoopCounter()
    {
        foreach (GameObject loopCountTextGameObject in loopCountTextGameObjects)
        {
            Destroy(loopCountTextGameObject);
        }
        loopCountTextGameObjects.Clear();
        loopCountTexts.Clear();
        currentLoopCount = 0;
    }
}
