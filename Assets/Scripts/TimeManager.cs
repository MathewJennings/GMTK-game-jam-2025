using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager instance;

    private float timeFraction;

    private float timer;

    public static void SetTimeScale(float newTimeFraction, float newDuration)
    {
        if (instance == null)
        {
            GameObject timeManagerObject = new GameObject("TimeManager");
            instance = timeManagerObject.AddComponent<TimeManager>();
            DontDestroyOnLoad(timeManagerObject);
        }

        instance.timeFraction = newTimeFraction;
        instance.timer = newDuration;

        Time.timeScale = newTimeFraction;
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            if (timer <= 0)
            {
                Time.timeScale = 1f; // Reset time scale
                Destroy(gameObject);
                instance = null;
            }
        }
    }
}
