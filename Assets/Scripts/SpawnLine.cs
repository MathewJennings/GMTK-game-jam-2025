using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnLine : MonoBehaviour
{
    [SerializeField]
    GameObject audioManager;

    [Header("Line Drawing Settings")]
    [SerializeField]
    GameObject linePrefab;

    [SerializeField]
    float lineTimeToFade;

    [SerializeField]
    int maxLineLength;

    [Header("Loop Counter Settings")]
    [SerializeField]
    float loopCounterOffsetX = 100f;

    [SerializeField]
    float loopCounterOffsetY = 50f;

    [SerializeField]
    int loopCounterFontSize = 24;

    [SerializeField]
    float loopCounterFadeDuration = 0.5f;

    [SerializeField]
    float loopCounterVerticalMovementAnimationDistance = 50f;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            GameObject line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

            LineDrawing lineManagement = line.GetComponent<LineDrawing>();
            lineManagement.SetTimeToFade(lineTimeToFade);
            lineManagement.SetMaxLineLength(maxLineLength);
            lineManagement.SetAudioManager(audioManager);

            LoopCounter loopCounter = line.GetComponent<LoopCounter>();
            loopCounter.SetDisplayOffset(loopCounterOffsetX, loopCounterOffsetY);
            loopCounter.SetFontSize(loopCounterFontSize);
            loopCounter.SetFadeDuration(loopCounterFadeDuration);
            loopCounter.SetVerticalMovementAnimationDistance(loopCounterVerticalMovementAnimationDistance);
        }
    }
}
