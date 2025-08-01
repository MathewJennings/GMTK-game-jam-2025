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

    private bool hasPreparedBossFight = false;

    void Awake()
    {
        if (spawnEnemy != null && currentLevel != null && waveAndBossBarsManager != null && bossProgressBar != null)
        {
            currentLevel.currentPoints = currentLevel.initialPointsBuffer;
            currentLevel.isBossFight = false;
            spawnEnemy.PlayLevel(currentLevel);
            waveAndBossBarsManager.SetWaveBarActive();
        }
        else
        {
            Debug.LogWarning("LevelManager: Missing reference to SpawnEnemy, StartingLevel, WaveAndBossBarsManager, or BossProgressBar.");
        }
    }

    void Update()
    {
        if (currentLevel.HasReachedTargetPoints() && !hasPreparedBossFight)
        {
            PrepareBossFight();
        }
        else if (currentLevel.HasRunOutOfPoints())
        {
            OnLoseConditionMet();
        }
    }

    private void PrepareBossFight()
    {
        currentLevel.isBossFight = true;
        GameObject boss = Instantiate(currentLevel.bossPrefab, Vector2.zero, Quaternion.identity);
        RandomMovement randomMovement = boss.AddComponent<RandomMovement>();
        randomMovement.InitializeBossPreset();
        waveAndBossBarsManager.SetBossBarActive();
        bossProgressBar.SetBossHealth(boss.GetComponent<BossHealth>());
        hasPreparedBossFight = true;
    }

    private void OnLoseConditionMet()
    {
        // winAndLoseUIManager.ShowLoseText();
    }

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