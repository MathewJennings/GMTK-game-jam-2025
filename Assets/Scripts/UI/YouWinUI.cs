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
    public Button restartPlusButton;
    public PickupSelector pickupSelector;

    [Header("Settings")]
    public string congratulationsMessage = "Congratulations!\nYou Win!";

    void Awake()
    {
        youWinPanel.SetActive(false);
        if (restartButton != null)
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        if (restartPlusButton != null)
        restartPlusButton.onClick.AddListener(OnNewGamePlusButtonClicked);
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
    }

    public void HideYouWinScreen()
    {
        youWinPanel.SetActive(false);
    }
    private void OnRestartButtonClicked()
    {
        pickupSelector.shouldResetPickups = true; 
        StartCoroutine(LoadSceneCoroutine());
    }
    
    private void OnNewGamePlusButtonClicked()
    {
        pickupSelector.shouldResetPickups = false;
        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("Tutorial");
    }
}