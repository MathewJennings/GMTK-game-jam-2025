using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private ScoreScriptableObject scoreScriptableObject;

    [Header("Win Condition Settings")]
    [SerializeField]
    private int targetScore = 1000;

    [SerializeField]
    private int initialScore = 20; // Creates a buffer for the constant decay of the score

    [SerializeField]
    private WinAndLoseUIManager winAndLoseUIManager;

    private void Awake()
    {
        scoreScriptableObject.currentScore = initialScore;
        scoreScriptableObject.targetScore = targetScore;
        scoreScriptableObject.hasWon = false;
        scoreScriptableObject.hasLost = false;
    }

    void Update()
    {
        if (scoreScriptableObject.currentScore >= scoreScriptableObject.targetScore)
        {
            OnWinConditionMet();
        }
        else if (scoreScriptableObject.currentScore <= 0)
        {
            OnLoseConditionMet();
        }
    }

    private void OnWinConditionMet()
    {
        scoreScriptableObject.hasWon = true;
        winAndLoseUIManager.ShowWinText();
    }

    private void OnLoseConditionMet()
    {
        scoreScriptableObject.hasLost = true;
        winAndLoseUIManager.ShowLoseText();
    }
}
