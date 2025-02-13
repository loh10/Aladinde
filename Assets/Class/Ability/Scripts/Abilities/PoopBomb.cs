using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "PoopBombAbility", menuName = "Scriptable Objects/Abilities/Poop Bomb")]
public class PoopBomb : Ability
{
    [SerializeField] private float maxRange = 5f;          // Maximum throw distance.
    [SerializeField] private float projectileSpeed = 10f;  // How fast the bomb travels.

    public override void Activate(GameObject user)
    {
        // Execute common ability logic (charge management, etc.)
        base.Activate(user);

        bool isBot = !user.CompareTag("Player");
        Vector2 userPos = user.transform.position;
        Vector2 explosionPosition;

        if (!isBot)
        {
            // Player: Calculate target position based on the mouse position
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorldPos - userPos;
            float travelDistance = Mathf.Min(direction.magnitude, maxRange);
            explosionPosition = userPos + direction.normalized * travelDistance;
        }
        else
        {
            // Bot: Target the enemy assigned in the behavior tree
            EnemyAI enemyAI = user.GetComponent<EnemyAI>();
            if (enemyAI != null && enemyAI._playerTarget != null)
            {
                Vector2 targetDirection = enemyAI._playerTarget.transform.position - user.transform.position;
                float travelDistance = Mathf.Min(targetDirection.magnitude, maxRange);
                explosionPosition = userPos + targetDirection.normalized * travelDistance;
            }
            else
            {
                Debug.LogWarning("Bot has no target, ability canceled.");
                return;
            }
        }

        // Always call the ServerRPC branch (using the computed explosion position).
        string prefabName = abilityPrefab.name;
        user.GetComponent<PlayerUseAbilities>().SpawnProjectileServerRpc(prefabName, userPos, explosionPosition, projectileSpeed);

        Debug.Log("Poop Bomb activated, explosion position: " + explosionPosition);
    }
}
