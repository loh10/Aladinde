using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "LeafThrowAbility", menuName = "Scriptable Objects/Abilities/Leaf Throw")]
public class LeafThrow : Ability
{
    [Header("Visual Effects")]
    [SerializeField] private GameObject attackVFXPrefab;

    public override void Activate(GameObject user)
    {
        // Execute common ability logic (charge management)
        base.Activate(user);
        bool isBot = user.gameObject.tag != "Player";
        Vector2 direction;

        if (!isBot)
        {
            // Get the current mouse position in world space.
            if (Camera.main != null)
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Calculate the normalized direction from the player to the mouse.
            direction = (mousePos - (Vector2)user.transform.position).normalized;
        }
        else
        {
            // Get the target from the behavior tree
            EnemyAI enemyAI = user.GetComponent<EnemyAI>();
            if (enemyAI != null && enemyAI._playerTarget != null)
            {
                direction = ((Vector2)enemyAI._playerTarget.transform.position - (Vector2)user.transform.position).normalized;
            }
            else
            {
                Debug.LogWarning("Bot has no target, GrillStrike cannot be executed correctly.");
                return;
            }
        }

        Collider2D userCollider = user.GetComponent<Collider2D>();
        float offset = userCollider != null ? userCollider.bounds.extents.magnitude + 0.1f : 0.5f;
        Vector2 rayStart = (Vector2)user.transform.position + direction * offset;
        
        // Cast a ray from the offset position.
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayStart, direction, range, Physics2D.DefaultRaycastLayers);
        
        // Debug log each hit.
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                Debug.Log("LeafThrow hit: " + hit.collider.gameObject.name);
            }
        }
        
        // Spawn the visual effect on all clients.
        if (attackVFXPrefab != null)
        {
            ulong ownerId = user.GetComponent<NetworkObject>().OwnerClientId;
            float simpleAttackDuration = 1.5f;
            user.GetComponent<PlayerUseAbilities>().RequestSpawnVFXServerRpc(attackVFXPrefab.name, rayStart, ownerId, simpleAttackDuration);
        }
        
        // Iterate over all hits and apply damage to those not in the user's hierarchy.
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && !hit.collider.transform.IsChildOf(user.transform))
            {
                NetworkObject netObj = hit.collider.transform.root.GetComponent<NetworkObject>();
                if (netObj != null)
                {
                    user.GetComponent<PlayerLifeManager>().TakeDamageServerRpc(damages, netObj.OwnerClientId);
                }
            }
        }
        
        Debug.Log(abilityName + " activated");
    }
}
