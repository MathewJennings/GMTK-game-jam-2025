using UnityEngine;

public class GravityWellLoopable : MonoBehaviour, ILoopable
{
    [SerializeField]
    private GravityWellSuck gravityWellSuck;
    
    private bool isPickupScene = false;
    
    void Awake()
    {
        isPickupScene = GameObject.Find("/SelectPickupManager") != null;
    }
    public LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        if (isPickupScene)
        {
            return new LoopResult(0, "Unlocked gravity well!", Color.purple, transform.position);
        }

        gravityWellSuck.Activate();
        Destroy(gameObject);
        return new LoopResult(0, "Gravity Well activated!", Color.purple, transform.position);
    }

}
