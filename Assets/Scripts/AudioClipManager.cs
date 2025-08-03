using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipManager : MonoBehaviour, ILineDrawingObserver, ILineBreakingObserver, ILoopObserver, IBossObserver
{
    [SerializeField]
    private AudioClip completedLoopClip;

    [SerializeField]
    private AudioClip brokenLineClip;

    [SerializeField]
    private AudioClip drawingLineClip;

    [SerializeField]
    private AudioClip bossSpawnedClip;


    [SerializeField]
    private AudioClip bossDefeatedClip;


    [SerializeField]
    private AudioClip lineSnappingBackClip;

    [Header("Background Music")]

    [SerializeField]
    private float crossfadeDuration = 2.0f;

    [SerializeField]
    private List<AudioClip> enemyWaveBackgroundMusicTracks;

    [SerializeField]
    private List<float> enemyWaveBackgroundMusicClipVolumes;

    [SerializeField]
    private List<AudioClip> bossBackgroundMusicTracks;

    [SerializeField]
    private List<float> bossBackgroundMusicClipVolumes;

    public static AudioClipManager Instance { get; private set; }

    private AudioSource soundEffectAudioSource;
    private AudioSource lineDrawingAudioSource;
    private AudioSource waveBackgroundMusicAudioSource;
    private AudioSource bossBackgroundMusicAudioSource;

    void Awake()
    {
        if (Instance == null)
        {
            InstantiateAudioClipManager();
        }
        else
        {
            Destroy(this.gameObject);
        }
        RegisterObservers();
    }

    private void InstantiateAudioClipManager()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        AudioSource[] audioSources = GetComponents<AudioSource>();
        soundEffectAudioSource = audioSources[0];

        lineDrawingAudioSource = audioSources[1];
        lineDrawingAudioSource.loop = true;
        lineDrawingAudioSource.clip = drawingLineClip;
        lineDrawingAudioSource.volume = 0.2f;

        waveBackgroundMusicAudioSource = audioSources[2];
        bossBackgroundMusicAudioSource = audioSources[3];
        waveBackgroundMusicAudioSource.loop = true;
        bossBackgroundMusicAudioSource.loop = true;
        waveBackgroundMusicAudioSource.volume = 0.5f;
        bossBackgroundMusicAudioSource.volume = 0.5f;
        waveBackgroundMusicAudioSource.clip = enemyWaveBackgroundMusicTracks[0];
        bossBackgroundMusicAudioSource.clip = bossBackgroundMusicTracks[0];
        waveBackgroundMusicAudioSource.Play();
        bossBackgroundMusicAudioSource.Stop();
    }

    private void RegisterObservers()
    {
        SpawnLine spawnLine = FindFirstObjectByType<SpawnLine>();
        spawnLine.RegisterLineDrawingObserver(Instance);
        spawnLine.RegisterLineBreakingObserver(Instance);
        spawnLine.RegisterLoopObserver(Instance);
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        levelManager.RegisterBossObserver(Instance);
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

    public void NotifyLineDrawingStarted()
    {
        lineDrawingAudioSource.Play();
    }

    public void NotifyLineDrawingEnded(int numPoints)
    {
        lineDrawingAudioSource.Stop();
        if (numPoints > 10)
        {
            RandomizePitch(soundEffectAudioSource);
            soundEffectAudioSource.volume = 0.5f;
            soundEffectAudioSource.PlayOneShot(lineSnappingBackClip);
        }
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

    private void RandomizePitchAndVolume(AudioSource audioSource)
    {
        RandomizePitch(audioSource);
        audioSource.volume = Random.Range(0.8f, 1.0f);
    }

    private void RandomizePitch(AudioSource audioSource)
    {
        audioSource.pitch = Random.Range(0.99f, 1.01f);
    }

    public void NotifyBossSpawned()
    {
        soundEffectAudioSource.PlayOneShot(bossSpawnedClip);
        StartCoroutine(CrossfadeMusic(waveBackgroundMusicAudioSource, bossBackgroundMusicAudioSource, bossBackgroundMusicClipVolumes[0]));
    }

    public void NotifyBossDefeated()
    {
        soundEffectAudioSource.PlayOneShot(bossDefeatedClip);
        StartCoroutine(CrossfadeMusic(bossBackgroundMusicAudioSource, waveBackgroundMusicAudioSource, enemyWaveBackgroundMusicClipVolumes[0]));
    }

    private IEnumerator CrossfadeMusic(AudioSource oldAudioSource, AudioSource newAudioSource, float targetVolume)
    {
        newAudioSource.volume = 0.0f;
        newAudioSource.Play();
        float timer = 0.0f;
        while (timer < crossfadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / crossfadeDuration;
            oldAudioSource.volume = targetVolume * (1.0f - t);
            newAudioSource.volume = targetVolume * t;
            yield return null;
        }
        oldAudioSource.volume = 0.0f;
        newAudioSource.volume = targetVolume;
        oldAudioSource.Stop();
    }
}
