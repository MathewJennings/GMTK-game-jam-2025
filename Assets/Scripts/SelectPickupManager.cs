using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectPickupManager : MonoBehaviour, ILoopObserver
{
    [SerializeField]
    private string levelScene;
    
    public PickupSelector pickupSelector; // Reference to the PickupSelector ScriptableObject
    public Transform pointA; // Position for the first pickup
    public Transform pointB; // Position for the second pickup
    
    private LevelManager levelManager; // Reference to the LevelManager

    void Start()
    {
        FindFirstObjectByType<SpawnLine>().RegisterLoopObserver(this);
        
        levelManager = FindObjectOfType<LevelManager>();

        // Get two random pickups that haven't been found
        List<GameObject> pickups = pickupSelector.GetUpToTwoRandomPickups();
        if (pickups == null || pickups.Count <= 0)
        {
            Debug.LogError("Not enough pickups available.");
            StartCoroutine(BeginNextLevelCoroutine());
        }

        // Instantiate the pickups at pointA and pointB
        Instantiate(pickups[0], pointA.position, Quaternion.identity);
        if (pickups.Count > 1)
        {
            Instantiate(pickups[1], pointB.position, Quaternion.identity);
        }
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

        // Begin next level
        StartCoroutine(BeginNextLevelCoroutine());
    }
     
    private IEnumerator BeginNextLevelCoroutine()
    {
        yield return new WaitForSeconds(2f);
        
        
        // Call PrepareCurrentLevel in LevelManager
        if (levelManager != null)
        {
            levelManager.PrepareCurrentLevel();
        }
        SceneManager.UnloadSceneAsync("SelectPickup");
    }
}
