using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid Prefabs")]
    public GameObject[] asteroidPrefabs; // Array of different asteroid prefab variants

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public float spawnHeight = 10f; // How far above the screen to spawn
    public float spawnRangeX = 15f; // X range for spawning (centered on spawner)
    public int minAsteroidsPerSpawn = 1; // Minimum asteroids to spawn at once
    public int maxAsteroidsPerSpawn = 3; // Maximum asteroids to spawn at once
    
    [Header("Scale Settings")]
    public float minScale = 0.5f;
    public float maxScale = 2f;

    [Header("Velocity Settings")]
    public float minFallSpeed = 1f;
    public float maxFallSpeed = 4f;
    public float minHorizontalDrift = -1f; // Small horizontal movement
    public float maxHorizontalDrift = 1f;

    [Header("Asteroid Settings")]
    public string asteroidTag = "LoopableObject";
    public int asteroidLayer = 0; // Default layer (Layer 0)

    private float timer = 0f;
    private Camera mainCamera;

    void Start()
    {
        // Get the main camera to calculate screen bounds
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }
        
        // Auto-calculate spawn range based on screen width if not manually set
        if (mainCamera != null && spawnRangeX <= 0)
        {
            float screenWorldWidth = mainCamera.orthographicSize * 2 * mainCamera.aspect;
            spawnRangeX = screenWorldWidth + 2f; // Add some padding
        }
        
        Debug.Log($"AsteroidSpawner initialized - SpawnRangeX: {spawnRangeX}, SpawnHeight: {spawnHeight}");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnAsteroidBatch();
            timer = 0f;
        }
    }

    void SpawnAsteroidBatch()
    {
        // Determine how many asteroids to spawn this time
        int asteroidsToSpawn = Random.Range(minAsteroidsPerSpawn, maxAsteroidsPerSpawn + 1);
        
        for (int i = 0; i < asteroidsToSpawn; i++)
        {
            SpawnAsteroid();
        }
        
        Debug.Log($"Spawned batch of {asteroidsToSpawn} asteroids");
    }

    void SpawnAsteroid()
    {
        // Check if we have asteroid prefabs to spawn
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0)
        {
            Debug.LogWarning("No asteroid prefabs assigned to AsteroidSpawner!");
            return;
        }

        // Pick a random asteroid prefab
        int randomIndex = Random.Range(0, asteroidPrefabs.Length);
        GameObject selectedPrefab = asteroidPrefabs[randomIndex];
        
        if (selectedPrefab == null)
        {
            Debug.LogWarning($"Asteroid prefab at index {randomIndex} is null!");
            return;
        }

        // Calculate spawn position: random X, above screen Y
        float randomX = transform.position.x + Random.Range(-spawnRangeX / 2f, spawnRangeX / 2f);
        float spawnY = transform.position.y + spawnHeight;
        Vector3 spawnPosition = new Vector3(randomX, spawnY, transform.position.z);

        // Instantiate the asteroid
        GameObject asteroid = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        
        // Set random scale
        float scale = Random.Range(minScale, maxScale);
        asteroid.transform.localScale = Vector3.one * scale;

        // Set tag and layer
        asteroid.tag = asteroidTag;
        asteroid.layer = asteroidLayer;

        // Add CircleBreaker component if it doesn't exist (for collision with drawn circles)
        if (asteroid.GetComponent<CircleBreaker>() == null)
        {
            asteroid.AddComponent<CircleBreaker>();
        }

        // Setup physics - make it fall down with optional horizontal drift
        Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Downward velocity with some horizontal drift
            float fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
            float horizontalDrift = Random.Range(minHorizontalDrift, maxHorizontalDrift);
            rb.linearVelocity = new Vector2(horizontalDrift, -fallSpeed);
            
            // Add some random rotation for visual variety
            rb.angularVelocity = Random.Range(-180f, 180f);
        }
        else
        {
            Debug.LogWarning($"Asteroid prefab {selectedPrefab.name} doesn't have a Rigidbody2D component!");
        }

        Debug.Log($"Spawned asteroid {selectedPrefab.name} at {spawnPosition} with scale {scale:F2}");
    }

    // Optional: Clean up asteroids that have fallen off screen
    void LateUpdate()
    {
        // Find and destroy asteroids that are too far below the screen
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag(asteroidTag);
        
        if (mainCamera != null)
        {
            float screenBottom = mainCamera.transform.position.y - mainCamera.orthographicSize - 5f; // 5 unit buffer
            
            foreach (GameObject asteroid in asteroids)
            {
                if (asteroid.transform.position.y < screenBottom)
                {
                    Debug.Log($"Cleaning up asteroid {asteroid.name} that fell off screen");
                    Destroy(asteroid);
                }
            }
        }
    }
}
