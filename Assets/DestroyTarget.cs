using UnityEngine;

public class DestroyTarget : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Target destroyed: " + other.gameObject.name);
        Destroy(other.gameObject);
    }
}
