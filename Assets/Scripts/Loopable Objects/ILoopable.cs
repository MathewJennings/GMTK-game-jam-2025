using UnityEngine;

public interface ILoopable
{
    /// <summary>
    /// Handle the looped event.
    /// </summary>
    /// <param name="line">The line GameObject that created the loop.</param>
    /// <returns>LoopResult containing score change and display text</returns>
    LoopResult HandleLooped(GameObject line, float multiplier);
}
