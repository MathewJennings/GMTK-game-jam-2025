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
            GameObject line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

            LineDrawing lineManagement = line.GetComponent<LineDrawing>();
            lineManagement.SetTimeToFade(lineTimeToFade);
            lineManagement.SetMaxLineLength(maxLineLength);
            lineManagement.SetAudioManager(audioManager);

            LineBreaker lineBreaker = line.GetComponent<LineBreaker>();
            lineBreaker.SetAudioManager(audioManager);

            LoopTextGenerator loopTextGenerator = canvas.GetComponent<LoopTextGenerator>();
            LoopCounter loopCounter = line.GetComponent<LoopCounter>();
            loopCounter.SetLoopTextGenerator(loopTextGenerator);

            LineGradient lineGradient = line.GetComponent<LineGradient>();
            lineGradient.SetLoopTextGenerator(loopTextGenerator);
        }
    }
}
