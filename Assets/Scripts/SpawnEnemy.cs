using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // Assign in Inspector

    private float timer = 0f;
    public float spawnInterval = 2f;

    [Header("Velocity Settings")]
    public float minVelocity = 1f; // Set in Inspector
    public float maxVelocity = 2f; // Set in Inspector

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
        Vector2 screenMin = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 screenMax = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        if (enemyPrefabs.Count > 0)
        {
            int side = Random.Range(0, 4); // 0 = top, 1 = bottom, 2 = left, 3 = right
            Vector2 spawnPosition = Vector2.zero;
            Vector2 targetPosition = Vector2.zero;
            switch (side)
            {
                case 0: // Top
                    spawnPosition = new Vector2(Random.Range(screenMin.x, screenMax.x), screenMax.y + 1);
                    targetPosition = Camera.main.ViewportToWorldPoint(
                        new Vector2(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.5f)));
                    break;
                case 1: // Bottom
                    spawnPosition = new Vector2(Random.Range(screenMin.x, screenMax.x), screenMin.y - 1);
                    targetPosition = Camera.main.ViewportToWorldPoint(
                        new Vector2(Random.Range(0.1f, 0.9f), Random.Range(0.5f, 0.9f)));
                    break;
                case 2: // Left
                    spawnPosition = new Vector2(screenMin.x - 1, Random.Range(screenMin.y, screenMax.y));
                    targetPosition = Camera.main.ViewportToWorldPoint(
                        new Vector2(Random.Range(0.5f, 0.9f), Random.Range(0.1f, 0.9f)));
                    break;
                case 3: // Right
                    spawnPosition = new Vector2(screenMax.x + 1, Random.Range(screenMin.y, screenMax.y));
                    targetPosition = Camera.main.ViewportToWorldPoint(
                        new Vector2(Random.Range(0.1f, 0.5f), Random.Range(0.1f, 0.9f)));
                    break;
            }

            int enemyIndex = Random.Range(0, enemyPrefabs.Count);
            // Instantiate the object
            GameObject obj = Instantiate(enemyPrefabs[enemyIndex], spawnPosition, Quaternion.identity);

            // Assign direction and speed
            TargetMover mover = obj.AddComponent<TargetMover>();
            mover.direction = (targetPosition - spawnPosition).normalized;
            mover.speed = Random.Range(minVelocity, maxVelocity);
        }
    }
}