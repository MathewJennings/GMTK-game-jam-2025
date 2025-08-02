using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectPickupManager : MonoBehaviour, ILoopObserver
{
    [SerializeField]
    private string levelScene;
    
    public PickupSelector pickupSelector; // Reference to the PickupSelector ScriptableObject
    public Transform pointA; // Position for the first pickup
    public Transform pointB; // Position for the second pickup
    public TextMeshProUGUI pickupAText; // Text field for the first pickup
    public TextMeshProUGUI pickupBText; // Text field for the second pickup

    private bool pickupSelected = false;
    private LevelManager levelManager; // Reference to the LevelManager
    private LoopTextGenerator loopTextGenerator;

    void Start()
    {
        FindFirstObjectByType<SpawnLine>().RegisterLoopObserver(this);
        WaveAndBossBarsManager waveAndBossBarsManager = FindFirstObjectByType<WaveAndBossBarsManager>();
        waveAndBossBarsManager.DisableBothBars();

        pickupSelected = false;
        levelManager = FindObjectOfType<LevelManager>();

        // Get the LoopTextGenerator from the Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            loopTextGenerator = canvas.GetComponent<LoopTextGenerator>();
            if (loopTextGenerator == null)
            {
                Debug.LogError("LoopTextGenerator not found on the Canvas.");
            }
        }

        // Get two random pickups that haven't been found
        List<GameObject> pickups = pickupSelector.GetUpToTwoRandomPickups();
        if (pickups == null || pickups.Count <= 0)
        {
            Debug.LogError("Not enough pickups available.");
            BeginNextLevel();
            return;
        }

        // Instantiate the pickups at pointA and pointB
        Instantiate(pickups[0], pointA.position, Quaternion.identity);
        pickupAText.text = pickupSelector.GetPickupDescription(pickups[0]);
        if (pickups.Count > 1)
        {
            Instantiate(pickups[1], pointB.position, Quaternion.identity);
            pickupBText.text = pickupSelector.GetPickupDescription(pickups[1]);
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
        if (pickupSelected)
        {
            Debug.Log("Returning");
            return;
        }

        int loopablesCount = line.GetComponent<LoopDetector>().GetLoopablesInLoop().Count;
        if (loopablesCount == 1)
        {
            Debug.Log("1 loopable");
            ILoopable loopable = line.GetComponent<LoopDetector>().GetLoopablesInLoop()[0];
            OnPickupSelected(loopable);
        } else if (loopablesCount > 1)
        {
            Debug.Log("2+ loopable");
            OnManyPickupsSelected(line, loopablesCount);
        }
    }

    private void OnManyPickupsSelected(GameObject line, int loopablesCount)
    {
        // If there are multiple loopables, show a message.
        string message = "Pick one only";
        if (loopTextGenerator != null)
        {
            Vector2 textPosition = Camera.main.WorldToScreenPoint(line.transform.position);
            loopTextGenerator.CreateLoopCountText(message, textPosition, Color.red);
        }
        else
        {
            Debug.LogWarning("LoopTextGenerator is not set. Cannot display selection message.");
        }
    }

    private void OnPickupSelected(ILoopable loopable)
    {
        // Find the matching pickupPrefab
        for (int i = 0; i < pickupSelector.pickupPrefabs.Count; i++)
        {
            // GetComponentInChildren checks both itself and its children. This is necessary for gravity well where the
            // loopable is actually on a child object.
            if (pickupSelector.pickupPrefabs[i].GetComponentInChildren(loopable.GetType()) != null)
            {
                // Mark the pickup as found
                pickupSelector.hasPickup[i] = true;
                break;
            }
        }

        pickupSelected = true;

        // Begin next level
        StartCoroutine(BeginNextLevelCoroutine());
    }
     
    private IEnumerator BeginNextLevelCoroutine()
    {
        yield return new WaitForSeconds(2f);
        BeginNextLevel();
    }

    private void BeginNextLevel()
    {
        // Call PrepareCurrentLevel in LevelManager
        if (levelManager != null)
        {
            levelManager.PrepareCurrentLevel();
        }
        SceneManager.UnloadSceneAsync("SelectPickup");
    }
}
