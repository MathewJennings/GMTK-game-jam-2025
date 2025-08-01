using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelManager : MonoBehaviour
{
    public LevelScriptableObject currentLevel; // Assign in Inspector
    public SpawnEnemy spawnEnemy; // Assign in Inspector

    void Awake()
    {
        if (spawnEnemy != null && currentLevel != null)
        {
            spawnEnemy.PlayLevel(currentLevel);
            currentLevel.currentPoints = currentLevel.initialPointsBuffer;
        }
        else
        {
            Debug.LogWarning("LevelManager: Missing reference to SpawnEnemy or StartingLevel.");
        }
    }

    void Update()
    {
        if (currentLevel.HasReachedTargetPoints())
        {
            OnTargetPointsMet();
        }
        else if (currentLevel.HasRunOutOfPoints())
        {
            OnLoseConditionMet();
        }
    }

    private void OnTargetPointsMet()
    {
        currentLevel.isBossFight = true;
        currentLevel.SetPointsForStartOfBossFight();
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