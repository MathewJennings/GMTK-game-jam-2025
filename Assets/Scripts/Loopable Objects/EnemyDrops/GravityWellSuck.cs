using UnityEngine;

public class GravityWellSuck : MonoBehaviour
{
    public float pullStrength = 10f; // Set in Inspector

    private bool isActive = false;
    private float activeTimer = 0f;
    public float activeDuration = 5f;
    private GameObject[] loopables;

    private void Update()
    {
        if (isActive)
        {
            PullLoopableObjects();
            activeTimer += Time.deltaTime;
            if (activeTimer >= activeDuration)
            {
                isActive = false;
                Destroy(gameObject);
            }
        }
    }

    public void Activate()
    {
        isActive = true;
        activeTimer = 0f;
        loopables = GameObject.FindGameObjectsWithTag("LoopableObject");
    }

    private void PullLoopableObjects()
    {
        foreach (GameObject obj in loopables)
        {
            if (obj != null && obj != gameObject && !IsTeleportBossCoin(obj))
            {
                Vector2 direction = (transform.position - obj.transform.position).normalized;
                float moveStep = pullStrength * Time.deltaTime;
                obj.transform.position = Vector2.MoveTowards(
                    obj.transform.position,
                    transform.position,
                    moveStep
                );
            }
        }
    }

    private bool IsTeleportBossCoin(GameObject obj)
    {
        return obj.GetComponent<InfinityCoinLoopable>() != null;
    }
}
