using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "Scriptable Objects/LevelScriptableObject")]
public class LevelScriptableObject : ScriptableObject
{
    public string levelName;
    public LevelScriptableObject nextLevel;
    public AnimationCurve spawnInterval = AnimationCurve.Linear(0, 1, 1, 1);
    public float currentCorruption = 0;
    public float initialCorruption = 50;
    public float totalCorruption = 100;

    public List<GameObject> enemyPrefabs;
    public List<float> enemyPrefabWeights; // Parallel list to enemyPrefabs

    public GameObject bossPrefab;
    public List<GameObject> bossPrefabs;

    public bool HasClearedAllCorruption()
    {
        return currentCorruption <= 0;
    }

    public bool HasReachedMaxCorruption()
    {
        return currentCorruption >= totalCorruption;
    }

    public GameObject GetRandomBoss()
    {
        if (bossPrefabs == null || bossPrefabs.Count == 0)
        {
            Debug.LogWarning("No boss prefabs available.");
            return null;
        }

        int randomIndex = Random.Range(0, bossPrefabs.Count);
        return bossPrefabs[randomIndex];
    }
}
