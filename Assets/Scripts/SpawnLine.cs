using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpawnLine : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    GameObject audioManager;

    [Header("Line Drawing Settings")]
    [SerializeField]
    GameObject linePrefab;

    [SerializeField]
    float lineTimeToFade;

    [SerializeField]
    int maxLineLength;

    private GameObject currentLine;

    void Awake()
    {
        if (levelManager == null)
        {
            Debug.LogWarning("SpawnLine: Missing reference to LevelManager.");
        }
    }

    private void Update()
    {
        if (levelManager.currentLevel.HasRunOutOfPoints()) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

            LineDrawing lineManagement = currentLine.GetComponent<LineDrawing>();
            lineManagement.SetTimeToFade(lineTimeToFade);
            lineManagement.SetMaxLineLength(maxLineLength);
            lineManagement.SetAudioManager(audioManager);

            LineBreaker lineBreaker = currentLine.GetComponent<LineBreaker>();
            lineBreaker.SetAudioManager(audioManager);

            LoopTextGenerator loopTextGenerator = canvas.GetComponent<LoopTextGenerator>();
            LoopCounter loopCounter = currentLine.GetComponent<LoopCounter>();
            loopCounter.SetLoopTextGenerator(loopTextGenerator);

            LineGradient lineGradient = currentLine.GetComponent<LineGradient>();
            lineGradient.SetLoopTextGenerator(loopTextGenerator);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && currentLine != null)
        {
            currentLine.GetComponent<LineDrawing>().DestroyLine();
            currentLine = null;
        }
    }
}
