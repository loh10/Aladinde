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
        // Execute common ability logic (e.g., charge management)
        base.Activate(user);

        if (Camera.main != null)
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Vector2 direction = (mousePos - (Vector2)user.transform.position).normalized;

        // Explicitly include all layers in the raycast.
        RaycastHit2D hit = Physics2D.Raycast(user.transform.position, direction, range, Physics2D.AllLayers);

        if (hit && hit.collider.gameObject != user)
        {
            user.GetComponent<PlayerLifeManager>()
                .TakeDamageServerRpc(damages, hit.collider.GetComponent<NetworkObject>().OwnerClientId);

            PlayerMovement targetMovement = hit.collider.GetComponent<PlayerMovement>();
            if (targetMovement != null)
            {
                targetMovement.ApplyStaggerServerRpc(_staggerDuration, _staggerSpeedMultiplier);
            }
        }
        
        Debug.Log(abilityName + " activated");
    }
}