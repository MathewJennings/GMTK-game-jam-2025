using System;
using Unity.Behavior;
using UnityEngine;
using System.Collections;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Spiral", story: "shoot [numBullets] [bullets] in a spiral around [self] at [BulletSpeed]", category: "Action", id: "18afb628546d67276e892bd9ec08d088")]
public partial class SpiralAction : Action
{
    [SerializeReference] public BlackboardVariable<int> NumBullets;
    [SerializeReference] public BlackboardVariable<GameObject> Bullets;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> BulletSpeed;

    [SerializeField] public float delayBetweenBullets = 0.1f; // configurable delay

    private MonoBehaviour coroutineRunner;
    private Coroutine spiralCoroutine;

    protected override Status OnStart()
    {
        coroutineRunner = (Self.Value != null) ? Self.Value.GetComponent<MonoBehaviour>() : null;
        if (coroutineRunner == null || Bullets.Value == null || NumBullets.Value <= 0)
            return Status.Failure;

        spiralCoroutine = coroutineRunner.StartCoroutine(SpiralShoot());
        return Status.Running;
    }

    private IEnumerator SpiralShoot()
    {
        int numBullets = NumBullets.Value;
        GameObject bulletPrefab = Bullets.Value;
        float speed = BulletSpeed != null ? BulletSpeed.Value : 5f;
        Transform origin = Self.Value.transform;
        float spawnOffset = 1.0f;

        float angleStep = 720f / numBullets;
        float angle = 0f;

        for (int i = 0; i < numBullets; i++)
        {
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Vector3 spawnPos = origin.position + (Vector3)(dir * spawnOffset);

            GameObject bullet = UnityEngine.Object.Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * speed;
            }

            angle += angleStep;
            yield return new WaitForSeconds(delayBetweenBullets);
        }
        spiralCoroutine = null; // Mark coroutine as finished

    }

    protected override Status OnUpdate()
    {
        // If coroutine is still running, keep returning Running
        if (spiralCoroutine != null)
            return Status.Running;
        return Status.Success;
    }

    protected override void OnEnd()
    {
        if (coroutineRunner != null && spiralCoroutine != null)
        {
            coroutineRunner.StopCoroutine(spiralCoroutine);
            spiralCoroutine = null;
        }
    }
}

