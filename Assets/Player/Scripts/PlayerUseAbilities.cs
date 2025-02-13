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
    public GameObject _attackToSpawn;

    private void Start()
    {
        _playerInfos = GetComponent<PlayerInfos>();
        // Assumes the ultimate ability is always the last in the array.
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
    
    [ServerRpc(RequireOwnership = false)]
    public void SpawnProjectileServerRpc(string prefabName, Vector3 spawnPosition, Vector3 targetPosition, float projectileSpeed, ServerRpcParams rpcParams = default)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        if (prefab != null)
        {
            GameObject projectile = Instantiate(prefab, spawnPosition, Quaternion.identity);
            projectile.GetComponent<NetworkObject>()?.Spawn();

            switch (prefabName)
            {
                case "SpicyBombProjectile":
                    {
                        SpicyBombProjectile spicyProj = projectile.GetComponent<SpicyBombProjectile>();
                        if (spicyProj != null)
                        {
                            spicyProj.Initialize(targetPosition, projectileSpeed);
                            spicyProj.SetOwner(gameObject);
                            return;
                        }
                        break;
                    }
                case "PoopBombProjectile":
                    {
                        PoopBombProjectile poopProj = projectile.GetComponent<PoopBombProjectile>();
                        if (poopProj != null)
                        {
                            poopProj.Initialize(targetPosition, projectileSpeed);
                            poopProj.SetOwner(gameObject);
                            return;
                        }
                        break;
                    }
                case "CorrosiveSauceProjectile":
                    {
                        CorrosiveSauceProjectile corrosiveProj = projectile.GetComponent<CorrosiveSauceProjectile>();
                        if (corrosiveProj != null)
                        {
                            corrosiveProj.Initialize(targetPosition, projectileSpeed);
                            corrosiveProj.SetOwner(gameObject);
                            return;
                        }
                        break;
                    }
                default:
                    Debug.LogWarning("SpawnProjectileServerRpc: Unrecognized projectile prefab name: " + prefabName);
                    break;
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
    
    public void OnPoopAttack(InputAction.CallbackContext ctx)
    {
        Debug.Log("OnPoopAttack called, phase: " + ctx.phase);
        if (ctx.phase == InputActionPhase.Started)
        {
            Debug.Log("Poop Attack triggered");
            _playerInfos.characterClass.abilities[0].Activate(gameObject);
        }
    }
    
    // --- VFX RPC METHODS ---
    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnVFXServerRpc(string effectPrefabName, Vector3 position, ulong ownerClientId, float duration, ServerRpcParams rpcParams = default)
    {
        // On the server, call the client RPC so that all clients spawn the effect.
        SpawnVFXClientRpc(effectPrefabName, position, ownerClientId, duration);
    }

    [ClientRpc]
    public void SpawnVFXClientRpc(string effectPrefabName, Vector3 position, ulong ownerClientId, float duration)
    {
        GameObject effectPrefab = Resources.Load<GameObject>(effectPrefabName);
        if (effectPrefab != null)
        {
            // Instantiate the effect at the provided position.
            GameObject instance = Instantiate(effectPrefab, position, Quaternion.identity);

            // Find the player object with the matching ownerClientId.
            PlayerSpawn[] players = FindObjectsOfType<PlayerSpawn>();
            foreach (PlayerSpawn player in players)
            {
                NetworkObject netObj = player.GetComponent<NetworkObject>();
                if (netObj != null && netObj.OwnerClientId == ownerClientId)
                {
                    // Parent the effect to that player's transform.
                    instance.transform.SetParent(player.transform, worldPositionStays: true);
                    break;
                }
            }

            // Destroy the instance after 'duration' seconds if duration > 0.
            if (duration > 0f)
            {
                Destroy(instance, duration);
            }
        }
        else
        {
            Debug.LogWarning("Effect prefab not found: " + effectPrefabName);
        }
    }
}