using UnityEngine;

public struct LoopResult
{
    public int scoreChange;
    public string displayText;
    public Color color;
    public Vector3 position;

    public LoopResult(int scoreChange, string displayText, Color c, Vector3 position)
    {
        this.scoreChange = scoreChange;
        this.displayText = displayText;
        this.color = c;
        this.position = position;
    }
}
