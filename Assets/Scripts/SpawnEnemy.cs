using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum SpawnType
{
    Cluster,
    Converge,
    Random
}

public class SpawnEnemy : MonoBehaviour
{
    private float timer = 0f;
    private float curveTime = 0f;
    private AnimationCurve spawnIntervalCurve = AnimationCurve.Linear(0, 5, 10, 5);
    private HashSet<GameObject> activeEnemies = new();
    private bool isBossMode = false;
    LevelScriptableObject bossModeLevel;
    private bool paused = false;

    [Header("Velocity Settings")]
    [Space]
    [Tooltip("Minimum velocity for spawned enemies.")]
    public float minVelocity = 1f;
    [Tooltip("Maximum velocity for spawned enemies.")]
    public float maxVelocity = 2f;

    [Header("Spawn Offset Settings")]
    [Space]
    [Tooltip("Minimum offset distance from spawn position.")]
    public float minOffsetDistance = 2f;
    [Tooltip("Maximum offset distance from spawn position.")]
    public float maxOffsetDistance = 3f;

    [Header("Enemy Count Settings")]
    [Space]
    [Tooltip("Minimum number of enemies to spawn.")]
    public int minEnemies = 1;
    [Tooltip("Maximum number of enemies to spawn.")]
    public int maxEnemies = 5;

    [Header("Spawn Type Weights")]
    [Space]
    [Tooltip("Probability weight for Cluster spawn type.")]
    [Range(0f, 1f)] public float clusterWeight = 0.33f;
    [Tooltip("Probability weight for Converge spawn type.")]
    [Range(0f, 1f)] public float convergeWeight = 0.33f;
    [Tooltip("Probability weight for Random spawn type.")]
    [Range(0f, 1f)] public float randomWeight = 0.34f;

    private LevelScriptableObject currentLevel;

    void Update()
    {
        if (paused) return;
        if (isBossMode || !currentLevel.HasClearedAllCorruption())
        {
            SpawnEnemies();
        }
        else if (activeEnemies.Count > 0)
        {
            ClearRemainingEnemies();
        }
    }

    public void ClearRemainingEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
    }

    private void SpawnEnemies()
    {

        timer += Time.deltaTime;
        curveTime += Time.deltaTime;

        float currentInterval = spawnIntervalCurve.Evaluate(curveTime);
        //if currentInterval == 0, we don't spawn enemies
        if (currentInterval != 0 && timer >= currentInterval)
        {
            int numEnemies = isBossMode ? 1 : Random.Range(minEnemies, maxEnemies + 1); // Inclusive of maxEnemies
            SpawnType selectedType = GetWeightedSpawnType();
            SpawnTarget(numEnemies, selectedType);
            timer = 0f;
        }
    }

    public void PauseSpawning()
    {
        paused = true;
    }
    public void ResumeSpawning()
    {
        paused = false;
    }

    public void PlayLevel(LevelScriptableObject level, bool isBossMode = false)
    {
        if (!isBossMode)
        {
            currentLevel = level;
        }
        else
        {
            bossModeLevel = level;
        }
        this.isBossMode = isBossMode;
        if (isBossMode || currentLevel != null)
        {

            spawnIntervalCurve = level.spawnInterval;
            curveTime = 0f;
        }
    }

    public void SpawnRandomEnemy()
    {
        int numEnemies = isBossMode ? 1 : Random.Range(minEnemies, maxEnemies + 1); // Inclusive of maxEnemies
        SpawnTarget(numEnemies, SpawnType.Random);
    }

    SpawnType GetWeightedSpawnType()
    {
        float totalWeight = clusterWeight + convergeWeight + randomWeight;
        float rand = Random.Range(0f, totalWeight);

        if (rand < clusterWeight)
            return SpawnType.Cluster;
        else if (rand < clusterWeight + convergeWeight)
            return SpawnType.Converge;
        else
            return SpawnType.Random;
    }

    Vector2 GetSpawnPosition(int side, Vector2 screenMin, Vector2 screenMax)
    {
        switch (side)
        {
            case 0: // Top
                return new Vector2(Random.Range(screenMin.x, screenMax.x), screenMax.y + 1);
            case 1: // Bottom
                return new Vector2(Random.Range(screenMin.x, screenMax.x), screenMin.y - 1);
            case 2: // Left
                return new Vector2(screenMin.x - 1, Random.Range(screenMin.y, screenMax.y));
            case 3: // Right
                return new Vector2(screenMax.x + 1, Random.Range(screenMin.y, screenMax.y));
            default:
                return Vector2.zero;
        }
    }

    Vector2 GetTargetPosition(int side)
    {
        switch (side)
        {
            case 0: // Top
                return Camera.main.ViewportToWorldPoint(
                    new Vector2(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.5f)));
            case 1: // Bottom
                return Camera.main.ViewportToWorldPoint(
                    new Vector2(Random.Range(0.1f, 0.9f), Random.Range(0.5f, 0.9f)));
            case 2: // Left
                return Camera.main.ViewportToWorldPoint(
                    new Vector2(Random.Range(0.5f, 0.9f), Random.Range(0.1f, 0.9f)));
            case 3: // Right
                return Camera.main.ViewportToWorldPoint(
                    new Vector2(Random.Range(0.1f, 0.5f), Random.Range(0.1f, 0.9f)));
            default:
                return Vector2.zero;
        }
    }

    void SpawnTarget(int numEnemies, SpawnType spawnType)
    {
        Vector2 screenMin = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 screenMax = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        LevelScriptableObject level = isBossMode ? bossModeLevel : currentLevel;
        if (level.enemyPrefabs.Count > 0)
        {
            int side = Random.Range(0, 4); // 0 = top, 1 = bottom, 2 = left, 3 = right
            Vector2 spawnPosition = GetSpawnPosition(side, screenMin, screenMax);
            Vector2 targetPosition = GetTargetPosition(side);

            // weighted random selection
            float totalWeight = 0f;
            for (int i = 0; i < level.enemyPrefabWeights.Count; i++)
            {
                totalWeight += level.enemyPrefabWeights[i];
            }
            float randomValue = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            int enemyIndex = 0;
            for (int i = 0; i < level.enemyPrefabWeights.Count; i++)
            {
                cumulative += level.enemyPrefabWeights[i];
                if (randomValue <= cumulative)
                {
                    enemyIndex = i;
                    break;
                }
            }

            var baseVelocity = Random.Range(minVelocity, maxVelocity);

            for (int i = 0; i < numEnemies; i++)
            {
                switch (spawnType)
                {
                    case SpawnType.Cluster:
                        // All enemies spawn close together
                        break;
                    case SpawnType.Converge:
                        // Randomize the start point for each enemy
                        side = Random.Range(0, 4);
                        spawnPosition = GetSpawnPosition(side, screenMin, screenMax);
                        break;
                    case SpawnType.Random:
                        // Randomize the target for each enemy
                        targetPosition = GetTargetPosition(side);
                        break;
                }
                Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(minOffsetDistance, maxOffsetDistance);
                Vector2 actualSpawnPos = spawnPosition + offset;
                Vector2 actualTargetPos = targetPosition + offset;

                GameObject obj = Instantiate(level.enemyPrefabs[enemyIndex], actualSpawnPos, Quaternion.identity);
                activeEnemies.Add(obj);
                EnemyHealth enemyHealth = obj.GetComponent<EnemyHealth>();
                // currentLevel is hacked when isBossMode is true
                if (isBossMode)
                {
                    enemyHealth.SetBossMode(true);
                }
                else
                {
                    enemyHealth.SetCurrentLevel(currentLevel);
                }

                TargetMover mover = obj.AddComponent<TargetMover>();
                mover.direction = (actualTargetPos - actualSpawnPos).normalized;
                mover.speed = baseVelocity;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpawnEnemy))]
public class SpawnEnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpawnEnemy spawner = (SpawnEnemy)target;
        if (GUILayout.Button("Spawn Cluster"))
        {
            spawner.SpawnRandomEnemy();
        }
    }
}
#endif