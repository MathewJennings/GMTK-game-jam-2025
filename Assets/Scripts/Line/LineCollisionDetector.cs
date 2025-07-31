using UnityEngine;

public class LineCollisionDetector : MonoBehaviour
{
    private GameObject audioManager;

    /// <summary>
    /// Must set the audio manager after instantiating the line drawing for playing audio feedback when a loop is created.
    /// </summary>
    /// <param name="audioManager">The Audio Manager GameObject.</param>
    public void SetAudioManager(GameObject audioManager)
    {
        this.audioManager = audioManager;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("LoopableObject"))
        {
            BreakLoop();
        }
    }

    private void BreakLoop()
    {
        GetComponent<LineDrawing>().DestroyLine();
        audioManager.GetComponent<AudioClipManager>().PlayBrokenLoopClip();
        LoopDetector loopDetector = GetComponent<LoopDetector>();
        loopDetector.ClearLoopablesInLoop();
    }
}
