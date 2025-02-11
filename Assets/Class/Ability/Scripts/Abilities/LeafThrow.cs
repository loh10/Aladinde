using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "LeafThrowAbility", menuName = "Scriptable Objects/Abilities/Leaf Throw")]
public class LeafThrow : Ability
{
    public override void Activate(GameObject user)
    {
        // Execute common ability logic (e.g., charge management)
        base.Activate(user);
        
        // Convert the mouse position to world coordinates
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Calculate the normalized direction from the user's position toward the mouse position
        Vector2 direction = (mousePos - (Vector2)user.transform.position).normalized;
        
        // Cast a ray that retrieves all objects in the given direction up to 'range'
        RaycastHit2D[] hits = Physics2D.RaycastAll(user.transform.position, direction, range);
        
        // Iterate over all objects hit by the ray
        foreach (RaycastHit2D hit in hits)
        {
            // Ensure a collider was hit and that the hit object is not the user
            if (hit.collider != null && hit.collider.gameObject != user)
            {
                // Get the NetworkObject component from the hit object
                NetworkObject netObj = hit.collider.GetComponent<NetworkObject>();
                if (netObj != null)
                {
                    // Apply damage to the target using its OwnerClientId via the networked life manager
                    user.GetComponent<PlayerLifeManager>().TakeDamageServerRpc(damages, netObj.OwnerClientId);
                }
            }
        }
        
        // Log that the LeafThrow ability has been activated
        Debug.Log(abilityName + " activated");
    }
}
