using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelManager : MonoBehaviour
{
    public LevelScriptableObject currentLevel; // Assign in Inspector
    public SpawnEnemy spawnEnemy; // Assign in Inspector
    public WaveAndBossBarsManager waveAndBossBarsManager; // Assign in Inspector
    public BossProgressBar bossProgressBar; // Assign in Inspector
    [SerializeField]
    private GameObject gameOverCanvas; // Assign in Inspector

    void Awake()
    {
        if (IsFullyConfigured())
        {
            PrepareCurrentLevel();
        }
        else
        {
            Debug.LogWarning("LevelManager: Missing reference to SpawnEnemy, StartingLevel, WaveAndBossBarsManager, or BossProgressBar.");
        }
    }

    public void PrepareCurrentLevel()
    {
        ResetCurrentLevel();
        currentLevel.currentPoints = currentLevel.initialPointsBuffer;
        currentLevel.hasPreparedBossFight = false;
        currentLevel.hasCompletedBossFight = false;
        spawnEnemy.PlayLevel(currentLevel);
        waveAndBossBarsManager.SetWaveBarActive();
    }

    private void ResetCurrentLevel()
    {
        gameOverCanvas.SetActive(false);
        // Destroy all objects labeled "loopableObject"
        GameObject[] loopableObjects = GameObject.FindGameObjectsWithTag("LoopableObject");
        foreach (GameObject obj in loopableObjects)
        {
            Destroy(obj);
        }
    }

    private void PrepareNextLevel()
    {
        currentLevel = currentLevel.nextLevel;
        PrepareCurrentLevel();
    }

    void Update()
    {
        if (!IsFullyConfigured())
        {
            return;
        }
        if (currentLevel.HasReachedTargetPoints() && !currentLevel.hasPreparedBossFight)
        {
            PrepareBossFight();
        }
        else if (currentLevel.HasRunOutOfPoints())
        {
            OnLoseConditionMet();
        }
        else if (currentLevel.hasCompletedBossFight)
        {
            PrepareNextLevel();
        }
    }

    private bool IsFullyConfigured()
    {
        return spawnEnemy != null && currentLevel != null && waveAndBossBarsManager != null && bossProgressBar != null;
    }

    private void PrepareBossFight()
    {
        GameObject boss = Instantiate(currentLevel.bossPrefab, Vector2.zero, Quaternion.identity);
        boss.GetComponent<EnemyHealth>().SetCurrentLevel(currentLevel);
        waveAndBossBarsManager.SetBossBarActive();
        bossProgressBar.SetBossHealth(boss.GetComponent<BossHealth>());
        currentLevel.hasPreparedBossFight = true;
    }

    public void OnLoseConditionMet()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameManager: Game Over Canvas is not assigned.");
        }
    }

    /// <summary>
    /// Only for the unity editor?
    /// </summary>
    public void PlayNextLevel()
    {
        if (currentLevel != null && currentLevel.nextLevel != null && spawnEnemy != null)
        {
            spawnEnemy.PlayLevel(currentLevel.nextLevel);
            Debug.Log($"Playing next level: {currentLevel.nextLevel.levelName}");
        }
        else
        {
            Debug.LogWarning("LevelManager: Missing reference to nextLevel or SpawnEnemy.");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelManager manager = (LevelManager)target;
        if (GUILayout.Button("Play Next Level"))
        {
            manager.PlayNextLevel();
        }
    }
}
#endif