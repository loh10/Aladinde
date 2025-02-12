using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "GrillStrikeAbility", menuName = "Scriptable Objects/Abilities/Grill Strike")]
public class GrillStrike : Ability
{
    private float _currentTime;
    private bool _canUse;
    
    [Header("Grill Strike Settings")]
    [SerializeField] private float _staggerDuration = 0.5f;
    [SerializeField] private float _staggerSpeedMultiplier = 0.5f;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject attackVFXPrefab;

    public override void Activate(GameObject user)
    {
        // Execute common ability logic (charge management)
        base.Activate(user);

        // Get current mouse position in world space.
        if (Camera.main != null)
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate normalized direction from the player to the mouse.
        Vector2 direction = (mousePos - (Vector2)user.transform.position).normalized;

        // Offset the ray's start position so that the ray doesn't immediately hit the player's own colliders.
        Collider2D userCollider = user.GetComponent<Collider2D>();
        float offset = userCollider != null ? userCollider.bounds.extents.magnitude + 0.1f : 0.5f;
        Vector2 rayStart = (Vector2)user.transform.position + direction * offset;

        // Cast a ray using the default raycast layers.
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, range, Physics2D.DefaultRaycastLayers);

        // Log what we hit.
        if (hit.collider != null)
            Debug.Log("GrillStrike hit: " + hit.collider.gameObject.name);

        // Spawn the visual effect on all clients.
        if (attackVFXPrefab != null)
        {
            Vector3 vfxPosition = hit ? (Vector3)hit.point : (Vector3)rayStart;
            ulong ownerId = user.GetComponent<NetworkObject>().OwnerClientId;
            float simpleAttackDuration = 1.5f;
            user.GetComponent<PlayerUseAbilities>().RequestSpawnVFXServerRpc(attackVFXPrefab.name, vfxPosition, ownerId, simpleAttackDuration);
        }

        // If we hit an object that is not part of the user's own hierarchy...
        if (hit && !hit.collider.transform.IsChildOf(user.transform))
        {
            NetworkObject targetNetObj = hit.collider.transform.root.GetComponent<NetworkObject>();
            if (targetNetObj != null)
            {
                user.GetComponent<PlayerLifeManager>().TakeDamageServerRpc(damages, targetNetObj.OwnerClientId);
            }

            PlayerMovement targetMovement = hit.collider.GetComponent<PlayerMovement>();
            if (targetMovement != null)
            {
                targetMovement.ApplyStaggerServerRpc(_staggerDuration, _staggerSpeedMultiplier);
            }
        }

        Debug.Log(abilityName + " activated");
    }
}
