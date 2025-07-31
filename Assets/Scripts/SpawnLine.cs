using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnLine : MonoBehaviour
{
    [SerializeField]
    GameObject linePrefab;

    [SerializeField]
    float lineTimeToFade;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            GameObject line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            line.GetComponent<LineManagement>().SetTimeToFade(lineTimeToFade);
        }
    }
}
