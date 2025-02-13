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

        bool isBot = user.CompareTag("Bot");
        Vector2 userPos = user.transform.position;
        Vector2 targetPosition;

        if (!isBot)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorldPos - userPos;
            float travelDistance = Mathf.Min(direction.magnitude, maxRange);
            targetPosition = userPos + direction.normalized * travelDistance;
        }
        else
        {
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

        // Always request the server to spawn the projectile using the determined target.
        string prefabName = abilityPrefab.name;
        user.GetComponent<PlayerUseAbilities>().SpawnProjectileServerRpc(prefabName, userPos, targetPosition, projectileSpeed);

        Debug.Log(abilityName + " activated, target position: " + targetPosition);
    }
}
