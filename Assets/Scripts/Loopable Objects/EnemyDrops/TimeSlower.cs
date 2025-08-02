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
        if (isPickupScene)
        {
            return new LoopResult(0, "Unlocked time warp!", Color.blue, transform.position);
        }

        TimeManager.SetTimeScale(timeFraction, duration);
        Destroy(gameObject);
        return new LoopResult(0, "Time warp activated!", Color.blue, transform.position);
    }
}
