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

    public override void Activate(GameObject user)
    {
        // Execute common ability logic (charge management)
        base.Activate(user);

        // Get the current mouse position in world space.
        if (Camera.main != null)
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the normalized direction from the player to the mouse.
        Vector2 direction = (mousePos - (Vector2)user.transform.position).normalized;

        // Offset the ray's start position so it doesn't start inside any of the user's colliders.
        Collider2D userCollider = user.GetComponent<Collider2D>();
        float offset = userCollider != null ? userCollider.bounds.extents.magnitude + 0.1f : 0.5f;
        Vector2 rayStart = (Vector2)user.transform.position + direction * offset;

        // Use the default raycast layers (which ignore objects on the "Ignore Raycast" layer).
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, range, Physics2D.DefaultRaycastLayers);

        // Debug what we hit.
        if (hit.collider != null)
            Debug.Log("GrillStrike hit: " + hit.collider.gameObject.name);

        // Check if the hit object is not part of the user's hierarchy.
        if (hit && !hit.collider.transform.IsChildOf(user.transform))
        {
            NetworkObject targetNetObj = hit.collider.transform.root.GetComponent<NetworkObject>();
            if (targetNetObj != null)
            {
                user.GetComponent<PlayerLifeManager>()
                    .TakeDamageServerRpc(damages, targetNetObj.OwnerClientId);
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
