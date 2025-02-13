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

        bool isBot = user.CompareTag("Bot");
        Vector2 userPos = user.transform.position;
        Vector2 targetPosition;

        if (!isBot)
        {
            // Player: Calculate target position based on the mouse position
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorldPos - userPos;
            float travelDistance = Mathf.Min(direction.magnitude, maxRange);
            targetPosition = userPos + direction.normalized * travelDistance;
        }
        else
        {
            // Bot: Target the enemy assigned in the behavior tree
            EnemyAI enemyAI = user.GetComponent<EnemyAI>();
            if (enemyAI != null && enemyAI._playerTarget != null)
            {
                Vector2 targetDirection = enemyAI._playerTarget.transform.position - user.transform.position;
                float travelDistance = Mathf.Min(targetDirection.magnitude, maxRange);
                targetPosition = userPos + targetDirection.normalized * travelDistance;
            }
            else
            {
                Debug.LogWarning("Bot has no target, ability canceled.");
                return;
            }
        }
        // Always ask the server (via RPC) to spawn the projectile with the client-provided target.
        string prefabName = abilityPrefab.name;
        user.GetComponent<PlayerUseAbilities>().SpawnProjectileServerRpc(prefabName, userPos, targetPosition, projectileSpeed);

        Debug.Log("Corrosive Sauce activated, target position: " + targetPosition);
    }
}
