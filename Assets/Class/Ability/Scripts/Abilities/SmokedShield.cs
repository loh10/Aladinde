using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "SmokedShieldAbility", menuName = "Scriptable Objects/Abilities/Smoked Shield")]
public class SmokedShield : Ability
{
    public float duration;

    [Header("Visual Effects")]
    [SerializeField] private GameObject shieldVFXPrefab;

    public override void Activate(GameObject user)
    {
        // Execute common ability logic (charge management)
        base.Activate(user);

        // Activate the shield effect on the player's life manager.
        user.GetComponent<PlayerLifeManager>().ActiShield(duration);

        // Spawn the shield VFX using the generic method.
        if (shieldVFXPrefab != null)
        {
            ulong ownerId = user.GetComponent<NetworkObject>().OwnerClientId;
            // Use the 'duration' of the shield as the lifetime of the VFX.
            user.GetComponent<PlayerUseAbilities>().RequestSpawnVFXServerRpc(shieldVFXPrefab.name, user.transform.position, ownerId, duration);
        }

        Debug.Log(abilityName + " activated");
    }
}