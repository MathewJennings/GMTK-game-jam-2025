using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnLine : MonoBehaviour
{
    [SerializeField]
    GameObject linePrefab;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
