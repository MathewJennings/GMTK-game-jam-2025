using UnityEngine;

public class TimeSlower : MonoBehaviour, ILoopable
{
    [SerializeField]
    private float timeFraction = 0.2f;
    
    [SerializeField]
    private float duration = 2f;

    public LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        TimeManager.SetTimeScale(timeFraction, duration);
        Destroy(gameObject);
        return new LoopResult(0, "Time slowed!", Color.blue, transform.position);
    }
}
