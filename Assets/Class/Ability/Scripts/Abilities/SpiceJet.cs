using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "SpiceJetAbility", menuName = "Scriptable Objects/Abilities/Spice Jet")]
public class SpiceJet : Ability
{
    // Critical chance (20%) and multiplier (1.5x) as specified in the GDD.
    public float critChance = 0.2f;
    public float critMultiplier = 1.5f;
    
    public override void Activate(GameObject user)
    {
        // Execute common ability logic (e.g. ultimate charge handling)
        base.Activate(user);
        
        // Convert the mouse position to world space coordinates
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Determine the direction from the user toward the mouse position
        Vector2 direction = (mousePos - (Vector2)user.transform.position).normalized;
        
        // Cast a ray from the user's position in the calculated direction, limited to 'range'
        RaycastHit2D hit = Physics2D.Raycast(user.transform.position, direction, range);
        
        // If the ray hits something (and it isn’t the user itself)
        if (hit && hit.collider.gameObject != user)
        {
            // Start with the base damage value from the ability asset
            float finalDamage = damages;
            
            // Apply critical hit multiplier if a random value is less than the crit chance
            if (Random.value < critChance)
            {
                finalDamage *= critMultiplier;
                Debug.Log("Critical Hit!");
            }
            
            // Retrieve the NetworkObject from the target to get its OwnerClientId
            NetworkObject targetNetObj = hit.collider.GetComponent<NetworkObject>();
            if (targetNetObj != null)
            {
                // Use the PlayerLifeManager to apply damage via a ServerRPC
                user.GetComponent<PlayerLifeManager>().TakeDamageServerRpc(finalDamage, targetNetObj.OwnerClientId);
            }
        }
        
        Debug.Log(abilityName + " activated");
    }
}
