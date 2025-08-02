using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickupSelector", menuName = "Scriptable Objects/PickupSelector")]
public class PickupSelector : ScriptableObject
{
    public List<GameObject> pickupPrefabs;
    public List<bool> hasPickup;

    public List<GameObject> GetUpToTwoRandomPickups()
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

        // If there are two or fewer available pickups, return them all.
        if (availablePickups.Count <= 2)
        {
            return availablePickups;
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

    public bool IsPickupUnlocked(GameObject pickup)
    {
        for (int i = 0; i < pickupPrefabs.Count; i++)
        {
            if (pickupPrefabs[i] == pickup)
            {
                return hasPickup[i];
            }
        }
        return false;
    }

    public void ClearAllPickups()
    {
        for (int i = 0; i < hasPickup.Count; i++)
        {
            hasPickup[i] = false;
        }
    }
}
