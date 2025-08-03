using UnityEngine;

public class DestroyTarget : MonoBehaviour
{
    // Destroys any other GameObject that collides with this one
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject != this.gameObject)
        {
            Destroy(collider.gameObject);
        }
    }
}
