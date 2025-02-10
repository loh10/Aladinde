using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "SpicyBombAbility", menuName = "Scriptable Objects/Abilities/Spicy Bomb")]
public class SpicyBomb : Ability
{
    [SerializeField] private float maxRange = 5f;
    [SerializeField] private float projectileSpeed = 10f; // meters per second

    public override void Activate(GameObject user)
    {
        // Execute common ability logic (charge management, etc.)
        base.Activate(user);

        // Calculate target position from the client's mouse position.
        Vector2 userPos = user.transform.position;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPos - userPos;
        float travelDistance = Mathf.Min(direction.magnitude, maxRange);
        Vector2 targetPosition = userPos + direction.normalized * travelDistance;

        // Always request the server to spawn the projectile using the client-calculated target.
        string prefabName = abilityPrefab.name;
        user.GetComponent<PlayerUseAbilities>().SpawnProjectileServerRpc(prefabName, userPos, targetPosition, projectileSpeed);

        Debug.Log(abilityName + " activated");
    }

    private void SpawnProjectile(Vector2 spawnPos, Vector2 targetPosition)
    {
        if (abilityPrefab != null)
        {
            GameObject projectile = Object.Instantiate(abilityPrefab, spawnPos, Quaternion.identity);
            // Spawn the projectile as a networked object.
            projectile.GetComponent<NetworkObject>()?.Spawn();
            SpicyBombProjectile bombProj = projectile.GetComponent<SpicyBombProjectile>();
            if (bombProj != null)
            {
                bombProj.Initialize(targetPosition, projectileSpeed);
            }
            else
            {
                Debug.LogWarning("SpawnProjectile: Projectile prefab is missing a SpicyBombProjectile component!");
            }
        }
        else
        {
            Debug.LogWarning("SpawnProjectile: No projectile prefab assigned to SpicyBomb ability!");
        }
    }
}
