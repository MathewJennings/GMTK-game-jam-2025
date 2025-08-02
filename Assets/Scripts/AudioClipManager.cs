using UnityEngine;

public class AudioClipManager : MonoBehaviour, ILineDrawingObserver, ILineBreakingObserver, ILoopObserver
{
    [SerializeField]
    private AudioClip completedLoopClip;

    [SerializeField]
    private AudioClip brokenLineClip;

    [SerializeField]
    private AudioClip drawingLineClip;

    private AudioSource soundEffectAudioSource;
    private AudioSource lineDrawingAudioSource;

    void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        soundEffectAudioSource = audioSources[0];
        lineDrawingAudioSource = audioSources[1];
        lineDrawingAudioSource.loop = true;
        lineDrawingAudioSource.clip = drawingLineClip;
        lineDrawingAudioSource.volume = 0.1f;

        SpawnLine spawnLine = FindFirstObjectByType<SpawnLine>();
        spawnLine.RegisterLineDrawingObserver(this);
        spawnLine.RegisterLineBreakingObserver(this);
        spawnLine.RegisterLoopObserver(this);
    }

    void OnDestroy()
    {
        SpawnLine spawnLine = FindFirstObjectByType<SpawnLine>();
        if (spawnLine != null)
        {
            spawnLine.UnregisterLineDrawingObserver(this);
            spawnLine.UnregisterLineBreakingObserver(this);
            spawnLine.UnregisterLoopObserver(this);
        }
    }

    private void RandomizePitchAndVolume(AudioSource audioSource)
    {
        audioSource.pitch = Random.Range(0.99f, 1.01f);
        audioSource.volume = Random.Range(0.8f, 1.0f);
    }

    public void NotifyLineDrawingStarted()
    {
        lineDrawingAudioSource.Play();
    }

    public void NotifyLineDrawingEnded()
    {
        lineDrawingAudioSource.Stop();
    }

    public void NotifyLineBroke()
    {
        lineDrawingAudioSource.Stop();
        RandomizePitchAndVolume(soundEffectAudioSource);
        soundEffectAudioSource.PlayOneShot(brokenLineClip);
    }

    public void NotifyLoopCompleted(GameObject line)
    {
        if (line.GetComponent<LoopDetector>().GetLoopablesInLoop().Count > 0)
        {
            RandomizePitchAndVolume(soundEffectAudioSource);
            soundEffectAudioSource.PlayOneShot(completedLoopClip);
        }
    }
}
