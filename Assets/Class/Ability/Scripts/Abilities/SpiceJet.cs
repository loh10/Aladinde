using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "SpiceJetAbility", menuName = "Scriptable Objects/Abilities/Spice Jet")]
public class SpiceJet : Ability
{
    public float critChance = 0.2f;
    public float critMultiplier = 1.5f;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject attackVFXPrefab;
    
    public override void Activate(GameObject user)
    {
        // Execute common ability logic (ultimate charge handling)
        base.Activate(user);
        
        // Convert the mouse position to world space coordinates.
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Calculate the normalized direction from the user to the mouse.
        Vector2 direction = (mousePos - (Vector2)user.transform.position).normalized;
        
        // Offset the ray's start position.
        Collider2D userCollider = user.GetComponent<Collider2D>();
        float offset = userCollider != null ? userCollider.bounds.extents.magnitude + 0.1f : 0.5f;
        Vector2 rayStart = (Vector2)user.transform.position + direction * offset;
        
        // Cast a ray using the default raycast layers.
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, range, Physics2D.DefaultRaycastLayers);
        
        // Debug log what we hit.
        if (hit.collider != null)
            Debug.Log("SpiceJet hit: " + hit.collider.gameObject.name);
        
        // Spawn the visual effect on all clients.
        if (attackVFXPrefab != null)
        {
            Vector3 vfxPosition = hit ? (Vector3)hit.point : (Vector3)rayStart;
            ulong ownerId = user.GetComponent<NetworkObject>().OwnerClientId;
            float simpleAttackDuration = 1.5f;
            user.GetComponent<PlayerUseAbilities>().RequestSpawnVFXServerRpc(attackVFXPrefab.name, vfxPosition, ownerId, simpleAttackDuration);
        }
        
        if (hit && !hit.collider.transform.IsChildOf(user.transform))
        {
            float finalDamage = damages;
            if (Random.value < critChance)
            {
                finalDamage *= critMultiplier;
                Debug.Log("Critical Hit!");
            }
            
            NetworkObject targetNetObj = hit.collider.transform.root.GetComponent<NetworkObject>();
            if (targetNetObj != null)
            {
                user.GetComponent<PlayerLifeManager>().TakeDamageServerRpc(finalDamage, targetNetObj.OwnerClientId);
            }
        }
        
        Debug.Log(abilityName + " activated");
    }
}
