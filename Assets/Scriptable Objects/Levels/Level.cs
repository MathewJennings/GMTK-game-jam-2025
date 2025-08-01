using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "Scriptable Objects/LevelScriptableObject")]
public class LevelScriptableObject : ScriptableObject
{
    public string levelName;
    public LevelScriptableObject nextLevel;
    public AnimationCurve spawnInterval = AnimationCurve.Linear(0, 1, 1, 1);
    public float currentPoints = 20;
    public float targetPoints = 100;

    public bool HasReachedTargetPoints()
    {
        return currentPoints >= targetPoints;
    }

    public bool HasRunOutOfPoints()
    {
        return currentPoints <= 0;
    }
}
