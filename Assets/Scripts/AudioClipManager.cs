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
        audioSource.PlayOneShot(completedLoopClip);
    }

    public void PlayBrokenLoopClip()
    {
        audioSource.PlayOneShot(brokenLoopClip);
    }
}
