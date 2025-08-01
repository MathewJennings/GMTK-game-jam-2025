using UnityEngine;

[CreateAssetMenu(fileName = "ScoreScriptableObject", menuName = "Scriptable Objects/ScoreScriptableObject")]
public class ScoreScriptableObject : ScriptableObject
{
    public float currentScore = 0;
    public float targetScore = 1000;
    public bool hasWon = false;
    public bool hasLost = false;
}
