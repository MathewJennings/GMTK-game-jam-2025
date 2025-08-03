using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FireCannons", story: "Fires [numBullets] [bullets] at [bulletSpeed]", category: "Action", id: "fe1d5bdb89cfe14de3b67093672dd8d2")]
public partial class FireCannonsAction : Action
{
    [SerializeReference] public BlackboardVariable<int> NumBullets;
    [SerializeReference] public BlackboardVariable<GameObject> Bullets;
    [SerializeReference] public BlackboardVariable<float> BulletSpeed;

    protected override Status OnStart()
    {
        int numBullets = NumBullets.Value;
        GameObject bulletPrefab = Bullets.Value;
        float speed = BulletSpeed != null ? BulletSpeed.Value : 5f; // Default to 5 if not set

        if (bulletPrefab == null || numBullets <= 0)
            return Status.Failure;

        Camera cam = Camera.main;
        float z = 0f;
        Vector3 left = cam.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, z));
        Vector3 right = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, z));
        Vector3 top = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height, z));
        Vector3 bottom = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2f, 0, z));

        // Pick a random side: 0=left, 1=right, 2=top, 3=bottom
        int side = UnityEngine.Random.Range(0, 4);

        for (int i = 0; i < numBullets; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            Vector2 velocity = Vector2.zero;

            switch (side)
            {
                case 0: // Left, fire right
                    spawnPos = cam.ScreenToWorldPoint(new Vector3(0, Mathf.Lerp(0, Screen.height, i / (float)(numBullets - 1)), z));
                    velocity = Vector2.right * speed;
                    break;
                case 1: // Right, fire left
                    spawnPos = cam.ScreenToWorldPoint(new Vector3(Screen.width, Mathf.Lerp(0, Screen.height, i / (float)(numBullets - 1)), z));
                    velocity = Vector2.left * speed;
                    break;
                case 2: // Top, fire down
                    spawnPos = cam.ScreenToWorldPoint(new Vector3(Mathf.Lerp(0, Screen.width, i / (float)(numBullets - 1)), Screen.height, z));
                    velocity = Vector2.down * speed;
                    break;
                case 3: // Bottom, fire up
                    spawnPos = cam.ScreenToWorldPoint(new Vector3(Mathf.Lerp(0, Screen.width, i / (float)(numBullets - 1)), 0, z));
                    velocity = Vector2.up * speed;
                    break;
            }

            spawnPos.z = 0f;
            GameObject bullet = UnityEngine.Object.Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            // Set bullet velocity if it has a Rigidbody2D
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = velocity;
            }
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

