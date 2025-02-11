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
}
