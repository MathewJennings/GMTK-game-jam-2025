using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnLine : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    Canvas canvas;

    [Header("Line Drawing Settings")]
    [SerializeField]
    GameObject linePrefab;

    [SerializeField]
    float lineTimeToFade;

    [SerializeField]
    float lineTimeToFadeWhenFinishedDrawing;

    [SerializeField]
    int maxLineLength;

    private readonly List<ILoopObserver> loopObservers = new();
    public void RegisterLoopObserver(ILoopObserver loopObserver) { loopObservers.Add(loopObserver); }
    public void UnregisterLoopObserver(ILoopObserver loopObserver) { loopObservers.Remove(loopObserver); }

    private readonly List<ILineDrawingObserver> lineDrawingObservers = new();
    public void RegisterLineDrawingObserver(ILineDrawingObserver lineDrawingObserver) { lineDrawingObservers.Add(lineDrawingObserver); }
    public void UnregisterLineDrawingObserver(ILineDrawingObserver lineDrawingObserver) { lineDrawingObservers.Remove(lineDrawingObserver); }

    private readonly List<ILineBreakingObserver> lineBreakingObservers = new();
    public void RegisterLineBreakingObserver(ILineBreakingObserver lineBreakingObserver) { lineBreakingObservers.Add(lineBreakingObserver); }
    public void UnregisterLineBreakingObserver(ILineBreakingObserver lineBreakingObserver) { lineBreakingObservers.Remove(lineBreakingObserver); }

    void Awake()
    {
        if (levelManager == null)
        {
            Debug.LogWarning("SpawnLine: Missing reference to LevelManager.");
        }
    }

    private void Update()
    {
        if (levelManager.currentLevel == null ||
            levelManager.currentLevel.HasRunOutOfPoints()) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            InitializeNewLine();
        }
    }

    private void InitializeNewLine()
    {
        GameObject line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        InitializeLineDrawing(line);
        InitializeLineBreaker(line);
        InitializeLoopDetector(line);

        LoopTextGenerator loopTextGenerator = canvas.GetComponent<LoopTextGenerator>();
        InitializeLoopCounter(line, loopTextGenerator);
        InitializeLineGradient(line, loopTextGenerator);
    }

    private void InitializeLineDrawing(GameObject line)
    {
        LineDrawing lineDrawing = line.GetComponent<LineDrawing>();
        lineDrawing.SetTimeToFade(lineTimeToFade);
        lineDrawing.SetTimeToFadeWhenFinishedDrawing(lineTimeToFadeWhenFinishedDrawing);
        lineDrawing.SetMaxLineLength(maxLineLength);
        lineDrawing.SetNotifyOnLineDrawingEnded((numPoints) =>
        {
            foreach (ILineDrawingObserver lineDrawingObserver in lineDrawingObservers)
            {
                lineDrawingObserver.NotifyLineDrawingEnded(numPoints);
            }
        });
        NotifyOnLineDrawingStarted();
    }

    private void NotifyOnLineDrawingStarted()
    {
        foreach (ILineDrawingObserver lineDrawingObserver in lineDrawingObservers)
        {
            lineDrawingObserver.NotifyLineDrawingStarted();
        }
    }

    private void InitializeLineBreaker(GameObject line)
    {
        LineBreaker lineBreaker = line.GetComponent<LineBreaker>();
        lineBreaker.SetNotifyOnLineBroke(() =>
        {
            foreach (ILineBreakingObserver lineBreakingObserver in lineBreakingObservers)
            {
                lineBreakingObserver.NotifyLineBroke();
            }
        });
    }

    private void InitializeLoopDetector(GameObject line)
    {
        LoopDetector loopDetector = line.GetComponent<LoopDetector>();
        loopDetector.SetNotifyOnLoopCompleted((gameObject) =>
        {
            foreach (ILoopObserver loopObserver in loopObservers)
            {
                loopObserver.NotifyLoopCompleted(gameObject);
            }
        });
    }

    private void InitializeLoopCounter(GameObject line, LoopTextGenerator loopTextGenerator)
    {
        LoopCounter loopCounter = line.GetComponent<LoopCounter>();
        loopCounter.SetLoopTextGenerator(loopTextGenerator);
    }

    private void InitializeLineGradient(GameObject line, LoopTextGenerator loopTextGenerator)
    {
        LineGradient lineGradient = line.GetComponent<LineGradient>();
        lineGradient.SetLoopTextGenerator(loopTextGenerator);
    }
}
