using UnityEngine;

public struct LoopResult
{
    public int scoreChange;
    public string displayText;
    public Vector3 position;

    public LoopResult(int scoreChange, string displayText, Vector3 position)
    {
        this.scoreChange = scoreChange;
        this.displayText = displayText;
        this.position = position;
    }
}
