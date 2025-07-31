using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpawnLine : MonoBehaviour
{
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

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            GameObject line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

            LineDrawing lineManagement = line.GetComponent<LineDrawing>();
            lineManagement.SetTimeToFade(lineTimeToFade);
            lineManagement.SetMaxLineLength(maxLineLength);
            lineManagement.SetAudioManager(audioManager);

            LineBreaker lineBreaker = line.GetComponent<LineBreaker>();
            lineBreaker.SetAudioManager(audioManager);
            
            LoopCounter loopCounter = line.GetComponent<LoopCounter>();
            loopCounter.SetCanvas(canvas);
        }
    }
}
