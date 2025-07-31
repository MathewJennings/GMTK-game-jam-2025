using UnityEngine;

public class LineCollisionDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("LoopableObject")) {
            GetComponent<LineDrawing>().DestroyLine();
        }
    }
}
