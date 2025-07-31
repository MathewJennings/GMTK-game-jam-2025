using UnityEngine;

public class LineTipCollisionDetector : MonoBehaviour
{
    [SerializeField]
    GameObject lineTipGameObject;

    private CircleCollider2D tipCollider;

    private void Awake() {
        tipCollider = lineTipGameObject.GetComponent<CircleCollider2D>();
    }

    public void SetTipPosition(Vector3 position) {
        lineTipGameObject.transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Tip collision detected: " + other.gameObject.name);
    }
}
