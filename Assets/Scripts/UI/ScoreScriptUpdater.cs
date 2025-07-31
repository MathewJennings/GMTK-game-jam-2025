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

    [SerializeField]
    private int targetScore = 1000;

    private void Awake()
    {
        scoreScriptableObject.score = 0;
    }

    void Update()
    {
        currentScoreText.text = "Score: " + scoreScriptableObject.score.ToString();
        targetScoreText.text = "Target: " + targetScore.ToString();
    }
}
