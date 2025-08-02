using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YouWinUI : MonoBehaviour
{
    [SerializeField]
    GameObject youWinPanel;

    [Header("UI Elements")]
    public Text congratulationsText;
    public Button restartButton;

    [Header("Settings")]
    public string congratulationsMessage = "Congratulations!\nYou Win!";

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
        youWinPanel.SetActive(true);
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