using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "Scriptable Objects/LevelScriptableObject")]
public class LevelScriptableObject : ScriptableObject
{
    public string levelName;
    public LevelScriptableObject nextLevel;
    public AnimationCurve spawnInterval = AnimationCurve.Linear(0, 1, 1, 1);
}
