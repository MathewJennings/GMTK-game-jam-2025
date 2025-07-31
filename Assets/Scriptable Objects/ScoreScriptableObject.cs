using UnityEngine;

[CreateAssetMenu(fileName = "ScoreScriptableObject", menuName = "Scriptable Objects/ScoreScriptableObject")]
public class ScoreScriptableObject : ScriptableObject
{
    public int currentScore = 0;
    public int targetScore = 1000;
    public bool hasWon = false;
}
