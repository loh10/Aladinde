using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeManager : NetworkBehaviour
{
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Slider _shieldBar;

    private float _maxHealth;
    private float _maxShield;
    public bool isBurning = false;

    private NetworkVariable<float> _currentHealth = new NetworkVariable<float>();
    private NetworkVariable<float> _currentShield = new NetworkVariable<float>();


    public override void OnNetworkSpawn()
    {
        _maxHealth = GetComponent<PlayerInfos>().characterClass.maxHealth;
        _healthBar.maxValue = _maxHealth;
        _healthBar.value = _maxHealth;
        _currentHealth = new NetworkVariable<float>(_maxHealth);


        _maxShield = GetComponent<PlayerInfos>().characterClass.maxShield;
        _shieldBar.maxValue = _maxShield;
        _shieldBar.value = 0;
    }

    public void ActiShield(float duration)
    {
        StartCoroutine(ActivateShield(duration));
    }

    private IEnumerator ActivateShield(float duration)
    {
        SetShieldServerRpc(_maxShield, OwnerClientId);
        yield return new WaitForSeconds(duration);
        SetShieldServerRpc(0, OwnerClientId);
    }

    public IEnumerator ApplyBurn(float burnDamage, float duration)
    {
        if (isBurning) yield break;
        isBurning = true;

        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            TakeDamageServerRpc(burnDamage, OwnerClientId);
            timeElapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        isBurning = false;
    }

    public void ReduceShield(float amount)
    {
        _currentShield = new NetworkVariable<float>(Mathf.Max(0, _currentShield.Value - amount));
    }
    [ServerRpc]
    public void TakeDamageServerRpc(float damage, ulong targetClientId)
    {
        PlayerLifeManager[] players = FindObjectsByType<PlayerLifeManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (player.OwnerClientId == targetClientId)
            {
                if (player._currentShield.Value > 0)
                {
                    float shieldDamage = Mathf.Min(player._currentShield.Value, damage);
                    player._currentShield.Value -= shieldDamage;
                    player.UpdateShieldBarClientRpc(player._currentShield.Value);
                    damage -= shieldDamage;
                }

                if (damage > 0)
                {
                    player._currentHealth.Value -= damage;
                    player.UpdateHealthBarClientRpc(player._currentHealth.Value);
                }

                if (player._currentHealth.Value <= 0)
                {
                    //TODO: BackToLobby();
                }
            }
        }
    }

    [ServerRpc]
    void SetShieldServerRpc(float amount, ulong targetClientId)
    {
        PlayerLifeManager[] players = FindObjectsByType<PlayerLifeManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (player.OwnerClientId == targetClientId)
            {
                player._currentShield.Value = amount;
                player.UpdateShieldBarClientRpc(player._currentShield.Value);
            }
        }
    }

    [ClientRpc]
    void UpdateHealthBarClientRpc(float newHealth)
    {
        _healthBar.value = newHealth;
    }

    [ClientRpc]
    void UpdateShieldBarClientRpc(float newShield)
    {
        _shieldBar.value = newShield;
    }
}