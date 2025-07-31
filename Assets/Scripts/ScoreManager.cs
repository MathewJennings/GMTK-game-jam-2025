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
    private string nextSceneName = "NextLevel";

    [SerializeField]
    private float nextSceneLoadDelay = 2f;

    private void Awake()
    {
        scoreScriptableObject.currentScore = 0;
        scoreScriptableObject.targetScore = targetScore;
        scoreScriptableObject.hasWon = false;
    }

    void Update()
    {
        if (scoreScriptableObject.currentScore >= scoreScriptableObject.targetScore)
        {
            OnWinConditionMet();
        }
    }

    private void OnWinConditionMet()
    {
        scoreScriptableObject.hasWon = true;
        Invoke(nameof(LoadNextScene), nextSceneLoadDelay);
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }
}
