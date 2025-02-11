using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "PoopBombAbility", menuName = "Scriptable Objects/Abilities/Poop Bomb")]
public class PoopBomb : Ability
{
    [SerializeField] private float maxRange = 5f;          // Maximum throw distance.
    [SerializeField] private float projectileSpeed = 10f;  // How fast the bomb travels.

    public override void Activate(GameObject user)
    {
        // Execute common ability logic (charge management, etc.)
        base.Activate(user);

        // Calculate the explosion position from the client's mouse position.
        Vector2 userPos = user.transform.position;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPos - userPos;
        float travelDistance = Mathf.Min(direction.magnitude, maxRange);
        Vector2 explosionPosition = userPos + direction.normalized * travelDistance;

        // Always call the ServerRPC branch (using the client's computed explosion position).
        string prefabName = abilityPrefab.name;
        user.GetComponent<PlayerUseAbilities>().SpawnProjectileServerRpc(prefabName, userPos, explosionPosition, projectileSpeed);

        Debug.Log("Poop Bomb activated, explosion position: " + explosionPosition);
    }
}
