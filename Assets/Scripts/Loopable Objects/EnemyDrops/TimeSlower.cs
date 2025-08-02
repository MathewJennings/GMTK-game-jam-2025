using UnityEngine;

public class TimeSlower : MonoBehaviour, ILoopable
{
    [SerializeField]
    private float timeFraction = 0.2f;
    
    [SerializeField]
    private float duration = 2f;

    private bool isPickupScene = false;

    void Awake()
    {
        isPickupScene = GameObject.Find("/SelectPickupManager") != null;
    }

    public LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        TimeManager.SetTimeScale(timeFraction, duration);
        if (!isPickupScene)
        {
            Destroy(gameObject);
        }

        return new LoopResult(0, "Time slow activated!", Color.blue, transform.position);
    }
}
