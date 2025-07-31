using UnityEngine;

public class spawnTarget : MonoBehaviour
{
    public GameObject targetPrefab; // Assign in Inspector

    private float timer = 0f;
    public float spawnInterval = 2f;

    [Header("Scale Settings")]
    public float minScale = .2f;
    public float maxScale = 2f;

    [Header("Velocity Settings")]
    public float minVelocity = 10f; // Set in Inspector
    public float maxVelocity = 20f; // Set in Inspector

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTarget();
            timer = 0f;
        }
    }

    void SpawnTarget()
    {
        if (targetPrefab != null)
        {
            GameObject obj = Instantiate(targetPrefab, transform.position, Quaternion.identity);
            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale = Vector3.one * scale;

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                rb.linearVelocity = randomDirection * Random.Range(minVelocity, maxVelocity);
            }
        }
    }
}