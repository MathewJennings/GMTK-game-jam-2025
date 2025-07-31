using UnityEngine;
using UnityEngine.UI;

public class CircleDrawingUI : MonoBehaviour
{
    [Header("UI References")]
    public Text resultText;
    public Text centerScoreText; // New field for center score display
    public Button resetButton;
    public InteractiveCircleDrawer circleDrawer;
    
    void Start()
    {
        // Find components if not assigned
        if (resultText == null)
        {
            resultText = FindFirstObjectByType<Text>();
        }
        
        if (resetButton == null)
        {
            resetButton = FindFirstObjectByType<Button>();
        }
        
        if (circleDrawer == null)
        {
            circleDrawer = FindFirstObjectByType<InteractiveCircleDrawer>();
        }
        
        // Create center score text if it doesn't exist
        SetupCenterScoreText();
        
        // Set up button event
        if (resetButton != null && circleDrawer != null)
        {
            resetButton.onClick.AddListener(circleDrawer.ResetDrawing);
        }
        
        // Initialize text
        if (resultText != null)
        {
            resultText.text = "Click and drag to draw a circle";
        }
    }
    
    void SetupCenterScoreText()
    {
        if (centerScoreText == null)
        {
            // Create a new UI Text for center score if not assigned
            GameObject centerScoreObj = new GameObject("CenterScoreText");
            centerScoreText = centerScoreObj.AddComponent<Text>();
            
            // Find the canvas and parent to it
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                centerScoreObj.transform.SetParent(canvas.transform, false);
            }
            
            // Configure the text component
            centerScoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            centerScoreText.fontSize = 24;
            centerScoreText.fontStyle = FontStyle.Bold;
            centerScoreText.alignment = TextAnchor.MiddleCenter;
            centerScoreText.color = Color.white;
            
            // Configure RectTransform
            RectTransform rectTransform = centerScoreText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 100);
            rectTransform.anchoredPosition = Vector2.zero;
            
            // Initially hide it
            centerScoreObj.SetActive(false);
            
            // Assign to circle drawer
            if (circleDrawer != null)
            {
                circleDrawer.centerScoreText = centerScoreText;
            }
        }
    }
}
