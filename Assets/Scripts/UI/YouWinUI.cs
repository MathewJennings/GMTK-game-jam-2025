using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class YouWinUI : MonoBehaviour
{
    [SerializeField]
    GameObject youWinPanel;

    [Header("UI Elements")]
    public Text congratulationsText;
    public Button restartButton;

    [Header("Settings")]
    public string congratulationsMessage = "Congratulations!\nYou Win!";

    public TMP_Text statsSummaryText;
    void Awake()
    {
        youWinPanel.SetActive(false);
        if (restartButton != null)
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        if (congratulationsText != null)
        congratulationsText.text = congratulationsMessage;
    }

    public void ShowYouWinScreen()
    {
        // Set endTime in LogManager
        LogManager logManager = FindFirstObjectByType<LogManager>();
        if (logManager != null)
        {
            logManager.endTime = Time.time;
        }
        else
        {
            Debug.LogWarning("LogManager not found.");
        }

        youWinPanel.SetActive(true);
        statsSummaryText.text = logManager.PrintLog();
    }

    public void HideYouWinScreen()
    {
        youWinPanel.SetActive(false);
    }
    private void OnRestartButtonClicked()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("Tutorial");
    }
}