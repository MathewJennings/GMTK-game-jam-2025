using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectPickupManager : MonoBehaviour, ILoopObserver
{
    public PickupSelector pickupSelector; // Reference to the PickupSelector ScriptableObject
    public Transform pointA; // Position for the first pickup
    public Transform pointB; // Position for the second pickup

    private GameObject pickupAInstance;
    private GameObject pickupBInstance;

    void Start()
    {
        FindFirstObjectByType<SpawnLine>().RegisterLoopObserver(this);

        // Get two random pickups that haven't been found
        List<GameObject> pickups = pickupSelector.GetTwoRandomPickups();
        if (pickups == null || pickups.Count < 2)
        {
            Debug.LogError("Not enough pickups available.");
            return;
        }

        // Instantiate the pickups at pointA and pointB
        pickupAInstance = Instantiate(pickups[0], pointA.position, Quaternion.identity);
        pickupBInstance = Instantiate(pickups[1], pointB.position, Quaternion.identity);
    }

    private void OnDestroy() {
        SpawnLine spawnLine = FindFirstObjectByType<SpawnLine>();
        if (spawnLine != null) {
            spawnLine.UnregisterLoopObserver(this);
        }
    }

    public void NotifyLoopCompleted(GameObject line)
    {
        // TODO: Handle when multiple things are looped.
        if (line.GetComponent<LoopDetector>().GetLoopablesInLoop().Count == 1)
        {
            ILoopable loopable = line.GetComponent<LoopDetector>().GetLoopablesInLoop()[0];
            OnPickupSelected(loopable);
        }
    }

    public void OnPickupSelected(ILoopable loopable)
    {
        // Find the matching pickupPrefab
        for (int i = 0; i < pickupSelector.pickupPrefabs.Count; i++)
        {
            if (pickupSelector.pickupPrefabs[i].GetComponent(loopable.GetType()) != null)
            {
                // Mark the pickup as found
                pickupSelector.hasPickup[i] = true;
                break;
            }
        }

        // Load the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
