public interface ILineDrawingObserver
{
    void NotifyLineDrawingStarted();
    void NotifyLineDrawingEnded(int numPoints);
}