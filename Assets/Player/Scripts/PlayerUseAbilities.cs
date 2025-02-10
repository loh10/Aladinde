using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUseAbilities : NetworkBehaviour
{
    private PlayerInfos _playerInfos;
    private Ability _ultimate;

    private float _currentTime;
    private bool _canSimpleAttack;
    public GameObject _attackToSpawn; // (Not used in this example, but kept for consistency.)

    private void Start()
    {
        _playerInfos = GetComponent<PlayerInfos>();
        // Assume the ultimate ability is always the last in the array.
        _ultimate = _playerInfos.characterClass.abilities[_playerInfos.characterClass.abilities.Length - 1];

        // Reset charges on all abilities.
        foreach (Ability ability in _playerInfos.characterClass.abilities)
        {
            ability.ResetCharge();
        }
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        CheckAttack(ref _canSimpleAttack, _playerInfos.characterClass.abilities[1].cooldown);
    }

    private void CheckAttack(ref bool canAttack, float cooldown)
    {
        canAttack = _currentTime > cooldown;
    }

    // ServerRpc to spawn a networked projectile.
    [ServerRpc(RequireOwnership = false)]
    public void SpawnProjectileServerRpc(string prefabName, Vector3 spawnPosition, Vector3 targetPosition, float projectileSpeed, ServerRpcParams rpcParams = default)
    {
        // Load the prefab from the Resources folder.
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        if (prefab != null)
        {
            GameObject projectile = Instantiate(prefab, spawnPosition, Quaternion.identity);
            projectile.GetComponent<NetworkObject>()?.Spawn();

            // Call the initialization method if present.
            SpicyBombProjectile sp = projectile.GetComponent<SpicyBombProjectile>();
            if (sp != null)
            {
                sp.Initialize(targetPosition, projectileSpeed);
            }
            else
            {
                Debug.LogWarning("SpawnProjectileServerRpc: The projectile prefab does not have a SpicyBombProjectile component!");
            }
        }
        else
        {
            Debug.LogWarning("SpawnProjectileServerRpc: Could not find prefab with name: " + prefabName);
        }
    }

    public void OnSimpleAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && _canSimpleAttack)
        {
            Debug.Log("Simple Attack");
            // Activate a simple attack ability (for example, LeafThrow, etc.)
            _playerInfos.characterClass.abilities[1].Activate(gameObject);
            _currentTime = 0;
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            if (_ultimate != null && _ultimate.CanUseUltimate())
            {
                _ultimate.Activate(gameObject);
            }
            else
            {
                Debug.Log("Ultimate not ready!");
            }
        }
    }
}
