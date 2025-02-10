using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "CorrosiveSauceAbility", menuName = "Scriptable Objects/Abilities/Corrosive Sauce")]
public class CorrosiveSauce : Ability
{
    [SerializeField] private float maxRange = 6f;           // Maximum throw distance (you can adjust this).
    [SerializeField] private float projectileSpeed = 10f;   // How fast the projectile travels.

    public override void Activate(GameObject user)
    {
        // Execute common ability logic (e.g., charge management)
        base.Activate(user);

        // Calculate target position based on the client's mouse position.
        Vector2 userPos = user.transform.position;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPos - userPos;
        float travelDistance = Mathf.Min(direction.magnitude, maxRange);
        Vector2 targetPosition = userPos + direction.normalized * travelDistance;

        // Always ask the server (via RPC) to spawn the projectile with the client-provided target.
        string prefabName = abilityPrefab.name;
        user.GetComponent<PlayerUseAbilities>().SpawnProjectileServerRpc(prefabName, userPos, targetPosition, projectileSpeed);

        Debug.Log("Corrosive Sauce activated, target position: " + targetPosition);
    }

    private void SpawnProjectile(Vector2 spawnPos, Vector2 targetPosition, GameObject owner)
    {
        if (abilityPrefab != null)
        {
            GameObject projectile = Object.Instantiate(abilityPrefab, spawnPos, Quaternion.identity);
            projectile.GetComponent<NetworkObject>()?.Spawn();
            CorrosiveSauceProjectile csProj = projectile.GetComponent<CorrosiveSauceProjectile>();
            if (csProj != null)
            {
                csProj.Initialize(targetPosition, projectileSpeed);
                csProj.SetOwner(owner);
            }
            else
            {
                Debug.LogWarning("SpawnProjectile: Projectile prefab is missing a CorrosiveSauceProjectile component!");
            }
        }
        else
        {
            Debug.LogWarning("SpawnProjectile: No projectile prefab assigned to Corrosive Sauce ability!");
        }
    }
}
