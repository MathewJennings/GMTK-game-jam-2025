using UnityEngine;

public class LineBreaker : MonoBehaviour
{
    private GameObject audioManager;/// <summary>

    /// Must set the audio manager after instantiating the line drawing for playing audio feedback when a loop is created.
    /// </summary>
    /// <param name="audioManager">The Audio Manager GameObject.</param>
    public void SetAudioManager(GameObject audioManager)
    {
        this.audioManager = audioManager;
    }

    public void BreakLine()
    {
        audioManager.GetComponent<AudioClipManager>().PlayBrokenLoopClip();
        GetComponent<LineDrawing>().DestroyLine();
    }
}
