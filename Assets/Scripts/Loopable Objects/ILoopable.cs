public interface ILoopable
{
    /// <summary>
    /// Handle the looped event.
    /// Return the amount to increment or decrement the score.
    /// </summary>
    /// <param name="loopCount">The number of consecutive loops that have been just been completed.</param>
    int HandleLooped(int loopCount);
}
