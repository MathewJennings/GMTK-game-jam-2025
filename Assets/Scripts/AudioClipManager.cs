using UnityEngine;

public class AudioClipManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip completedLoopClip;

    [SerializeField]
    private AudioClip brokenLoopClip;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCompletedLoopClip()
    {
        RandomizePitchAndVolume();
        audioSource.PlayOneShot(completedLoopClip);
    }

    public void PlayBrokenLoopClip()
    {
        RandomizePitchAndVolume();
        audioSource.PlayOneShot(brokenLoopClip);
    }

    private void RandomizePitchAndVolume()
    {
        audioSource.pitch = Random.Range(0.99f, 1.01f);
        audioSource.volume = Random.Range(0.8f, 1.0f);
    }
}
