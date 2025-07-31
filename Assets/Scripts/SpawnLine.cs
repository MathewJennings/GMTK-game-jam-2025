using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnLine : MonoBehaviour
{
    [SerializeField]
    GameObject linePrefab;

    [SerializeField]
    float lineTimeToFade;

    [SerializeField]
    int maxLineLength;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            GameObject line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            LineManagement lineManagement = line.GetComponent<LineManagement>();
            lineManagement.SetTimeToFade(lineTimeToFade);
            lineManagement.SetMaxLineLength(maxLineLength);
        }
    }
}
