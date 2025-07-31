using TMPro;
using UnityEngine;

public class ScoreScriptUpdater : MonoBehaviour
{
    [SerializeField]
    private ScoreScriptableObject scoreScriptableObject;

    [SerializeField]
    private TextMeshProUGUI currentScoreText;

    [SerializeField]
    private TextMeshProUGUI targetScoreText;

    void Update()
    {
        if (scoreScriptableObject.hasWon)
        {
            currentScoreText.text = "Score: " + scoreScriptableObject.targetScore.ToString();
            currentScoreText.color = Color.green;
            targetScoreText.text = "YOU WIN!";
            targetScoreText.color = Color.green;
            return;
        }
        currentScoreText.text = "Score: " + scoreScriptableObject.currentScore.ToString();
        targetScoreText.text = "Target: " + scoreScriptableObject.targetScore.ToString();
    }
}
