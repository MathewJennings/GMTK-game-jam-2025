using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour, ILoopObserver
{
    [SerializeField]
    float maxStrength = 1.2f;

    [SerializeField]
    float dampenDuration = 0.15f;

    private float currentStrength = 0.0f;
    private Coroutine dampenCoroutine;

    private void Start() {
        FindFirstObjectByType<SpawnLine>().RegisterLoopObserver(this);
    }

    private void OnDestroy() {
        SpawnLine spawnLine = FindFirstObjectByType<SpawnLine>();
        if (spawnLine != null) {
            spawnLine.UnregisterLoopObserver(this);
        }
    }

    public void NotifyLoopCompleted(GameObject line)
    {
        if (line.GetComponent<LoopDetector>().GetLoopablesInLoop().Count > 0)
        {
            currentStrength = maxStrength;
            if (dampenCoroutine != null)
            {
                StopCoroutine(dampenCoroutine);
            }
            dampenCoroutine = StartCoroutine(DampenStrength(line.GetComponent<LoopCounter>().GetCurrentLoopCount()));
        }
    }

    private void Update() {
        if (currentStrength >0f)
        {
        float randomX = Random.value - 0.5f;
            float randomY = Random.value - 0.5f;
            float randomZ = Random.value - 0.5f;
            transform.localEulerAngles = new Vector3(randomX, randomY, randomZ) * currentStrength;
        }
    }

    private IEnumerator DampenStrength(int loopCount)
    {
        float startStrength = currentStrength;
        float elapsedTime = 0f;
        float dampenDuration = this.dampenDuration + 0.05f*loopCount;
        while (elapsedTime < dampenDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dampenDuration;
            currentStrength = Mathf.Lerp(startStrength, 0f, t);
            yield return null;
        }
        currentStrength = 0f;
        dampenCoroutine = null;
    }
}
