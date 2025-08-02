using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickupSelector", menuName = "Scriptable Objects/PickupSelector")]
public class PickupSelector : ScriptableObject
{
    public List<GameObject> pickupPrefabs;
    public List<bool> hasPickup;

    public List<GameObject> GetTwoRandomPickups()
    {
        List<GameObject> availablePickups = new List<GameObject>();

        // Collect pickups that haven't been found
        for (int i = 0; i < pickupPrefabs.Count; i++)
        {
            if (!hasPickup[i])
            {
                availablePickups.Add(pickupPrefabs[i]);
            }
        }

        // Ensure there are at least two available pickups
        if (availablePickups.Count < 2)
        {
            return null;
        }

        // Randomly select two pickups
        List<GameObject> selectedPickups = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            int randomIndex = Random.Range(0, availablePickups.Count);
            selectedPickups.Add(availablePickups[randomIndex]);
            availablePickups.RemoveAt(randomIndex);
        }

        return selectedPickups;
    }
    
    public void MarkAsFound(string pickupName)
    {
        for (int i = 0; i < pickupPrefabs.Count; i++)
        {
            if (pickupPrefabs[i].name == pickupName)
            {
                hasPickup[i] = true;
                return;
            }
        }
        Debug.LogWarning($"Pickup '{pickupName}' not found in the list.");
    }
}
