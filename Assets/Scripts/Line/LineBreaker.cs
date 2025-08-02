using System;
using UnityEngine;

public class LineBreaker : MonoBehaviour
{
    private Action notifyOnLineBroke;
    public void SetNotifyOnLineBroke(Action notifyOnLineBroke)
    {
        this.notifyOnLineBroke = notifyOnLineBroke;
    }

    public void BreakLine()
    {
        GetComponent<LineDrawing>().DestroyLine();
        notifyOnLineBroke();
    }
}
