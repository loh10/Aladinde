using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "GrillStrikeAbility", menuName = "Scriptable Objects/Abilities/Grill Strike")]
public class GrillStrike : Ability
{
    private float _currentTime;
    private bool _canUse;
    
    [SerializeField] private float _staggerDuration = 0.5f;

    public override void Activate(GameObject user)
    {
        // Execute common ability logic (e.g., charge management)
        base.Activate(user);
        
        // Convert mouse position to world coordinates
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Calculate the normalized direction from the user to the mouse
        Vector2 direction = (mousePos - (Vector2)user.transform.position).normalized;
        
        // Cast a ray from the user's position in the calculated direction up to the defined range
        RaycastHit2D hit = Physics2D.Raycast(user.transform.position, direction, range);
        
        // If the ray hits an object that is not the user
        if (hit && hit.collider.gameObject != user)
        {
            // Apply damage to the hit target using its NetworkObject's OwnerClientId
            user.GetComponent<PlayerLifeManager>()
                .TakeDamageServerRpc(damages, hit.collider.GetComponent<NetworkObject>().OwnerClientId);
            
            // Get the target's PlayerMovement component.
            PlayerMovement targetMovement = hit.collider.GetComponent<PlayerMovement>();
            if (targetMovement != null)
            {
                // Instead of calling ApplyStagger directly, call the ServerRpc to propagate the effect.
                targetMovement.ApplyStaggerServerRpc(_staggerDuration);
            }
        }
        
        Debug.Log(abilityName + " activated");
    }
}