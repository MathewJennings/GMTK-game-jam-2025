using UnityEngine;

public class GravityWellLoopable : MonoBehaviour, ILoopable
{
    [SerializeField]
    private GravityWellSuck gravityWellSuck;
    public LoopResult HandleLooped(GameObject line, float multiplier = 1.0f)
    {
        gravityWellSuck.Activate();
        Destroy(gameObject);
        return new LoopResult(0, "Gravity Well!", Color.purple, transform.position);
    }

}
