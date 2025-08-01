using UnityEngine;

public class CircleAround : MonoBehaviour
{
    public Transform target;      // The object to circle around
    public float radius = 2f;     // Distance from the target
    public float speed = 1f;      // Speed of rotation (radians per second)
    public float fadeDuration = 1f;

    private float angle = 0f;
    private SpriteRenderer spriteRenderer;
    private float fadeTimer = 0f;
    private bool fadingIn = true;
    private bool fadingOut = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0f;
            spriteRenderer.color = c;
        }
        fadeTimer = 0f;
        fadingIn = true;
        fadingOut = false;
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        // Fade in
        if (fadingIn)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            SetAlpha(alpha);
            if (alpha >= 1f)
            {
                fadingIn = false;
            }
        }
        // Fade out
        else if (fadingOut)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (fadeTimer / fadeDuration));
            SetAlpha(alpha);
            if (alpha <= 0f)
            {
                Destroy(gameObject);
            }
        }

        // Start fade out if target is null
        if (!fadingOut && target == null)
        {
            fadingOut = true;
            fadeTimer = 0f;
        }

        // Only circle if not fading out and target exists
        if (!fadingOut && target != null)
        {
            angle += speed * Time.deltaTime;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            transform.position = target.position + new Vector3(x, y, 0f);
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}