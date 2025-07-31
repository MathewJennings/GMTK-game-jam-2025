using TMPro;
using UnityEngine;

public class ScoreScriptUpdater : MonoBehaviour
{
    [SerializeField]
    private ScoreScriptableObject scoreScriptableObject;

    private void Awake()
    {
        scoreScriptableObject.score = 0;
    }

    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = "Score: " + scoreScriptableObject.score.ToString();
    }
}
