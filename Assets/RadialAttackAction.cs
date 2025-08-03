using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RadialAttack", story: "shoot [numBullets] [bullets] in circle around [self] at speed [BulletSpeed]", category: "Action", id: "96c5a61459c9a8445a25bf971923c8f0")]
public partial class RadialAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<int> NumBullets;
    [SerializeReference] public BlackboardVariable<GameObject> Bullets;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> BulletSpeed;

    protected override Status OnStart()
    {
        // Get the number of bullets and the bullet prefab
        int numBullets = NumBullets.Value;
        GameObject bulletPrefab = Bullets.Value;
        float speed = BulletSpeed != null ? BulletSpeed.Value : 5f; // Default to 5 if not set
        Transform origin = Self.Value.transform;

        if (origin == null || bulletPrefab == null || numBullets <= 0)
            return Status.Failure;

        float angleStep = 360f / numBullets;
        float angle = 0f;
        float spawnOffset = 1.0f; // Distance from the boss to spawn bullets

        for (int i = 0; i < numBullets; i++)
        {
            // Calculate direction
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            // Calculate spawn position with offset
            Vector3 spawnPos = origin.position + (Vector3)(dir * spawnOffset);

            // Instantiate bullet at offset position
            GameObject bullet = UnityEngine.Object.Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            // Set bullet velocity if it has a Rigidbody2D
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * speed;
            }

            angle += angleStep;
        }

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

