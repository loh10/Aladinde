using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "SpiceJetAbility", menuName = "Scriptable Objects/Abilities/Spice Jet")]
public class SpiceJet : Ability
{
    public float critChance = 0.2f;
    public float critMultiplier = 1.5f;
    
    public override void Activate(GameObject user)
    {
        // Execute common ability logic (ultimate charge handling)
        base.Activate(user);

        // Convert the mouse position to world space coordinates.
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the normalized direction from the user to the mouse.
        Vector2 direction = (mousePos - (Vector2)user.transform.position).normalized;

        // Offset the ray start position.
        Collider2D userCollider = user.GetComponent<Collider2D>();
        float offset = userCollider != null ? userCollider.bounds.extents.magnitude + 0.1f : 0.5f;
        Vector2 rayStart = (Vector2)user.transform.position + direction * offset;

        // Cast the ray.
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, range, Physics2D.DefaultRaycastLayers);

        // Debug what we hit.
        if (hit.collider != null)
            Debug.Log("SpiceJet hit: " + hit.collider.gameObject.name);

        // If we hit an object that is not part of the user's hierarchy...
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