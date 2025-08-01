using UnityEngine;
using TMPro;

public class WinAndLoseUIManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI winText;

    [SerializeField]
    TextMeshProUGUI loseText;

    void Awake()
    {
        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
    }

    public void ShowWinText()
    {
        winText.gameObject.SetActive(true);
    }

    public void ShowLoseText()
    {
        loseText.gameObject.SetActive(true);
    }
}
