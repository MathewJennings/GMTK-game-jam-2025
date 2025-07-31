using UnityEngine;

public interface ILoopable
{
    /// <summary>
    /// Handle the looped event.
    /// Return the amount to increment or decrement the score.
    /// </summary>
    /// <param name="line">The line GameObject that created the loop.</param>
    int HandleLooped(GameObject line);
}
