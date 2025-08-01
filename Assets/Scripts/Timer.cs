using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timeText; // Assign in Inspector

    private float elapsedTime = 0f;

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (timeText != null)
        {
            timeText.text = $"Time: {elapsedTime:F2}s";
        }
    }
}
