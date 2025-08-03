using System;
using System.Collections;
using UnityEngine;

public class LineBreaker : MonoBehaviour
{
    private Action notifyOnLineBroke;
    private bool isGhostMode = false;

    public void SetNotifyOnLineBroke(Action notifyOnLineBroke)
    {
        this.notifyOnLineBroke = notifyOnLineBroke;
    }

    public void BreakLine()
    {
        if(isGhostMode)
        {
            return;
        }
        GetComponent<LineDrawing>().DestroyLine();
        notifyOnLineBroke();
    }

    public void SetGhostMode(float duration)
    {
        StartCoroutine(GhostModeCoroutine(duration));
    }

    private IEnumerator GhostModeCoroutine(float duration)
    {
        isGhostMode = true;
        yield return new WaitForSeconds(duration);
        isGhostMode = false;
    }
}
