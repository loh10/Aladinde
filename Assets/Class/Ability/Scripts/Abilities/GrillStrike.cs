using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GrillStrikeAbility", menuName = "Scriptable Objects/Abilities/Grill Strike")]
public class GrillStrike : Ability
{
    private float _currentTime;
    private bool _canUse;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)user.transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(user.transform.position, direction, range);
        if (hit)
        {
            if (hit.collider.gameObject != user)
            {
                user.GetComponent<PlayerLifeManager>().TakeDamageServerRpc(damages, hit.collider.GetComponent<NetworkObject>().OwnerClientId);
            }
        }
        
        Debug.Log(abilityName + " activated");
    }

}