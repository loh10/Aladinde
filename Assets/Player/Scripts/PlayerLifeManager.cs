using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeManager : NetworkBehaviour
{
    [SerializeField] private Slider _healthBar;

    private float _maxHealth;

    private NetworkVariable<float> _currentHealth = new NetworkVariable<float>();


    public override void OnNetworkSpawn()
    {
        _maxHealth = GetComponent<PlayerInfos>().characterClass.maxHealth;
        _healthBar.maxValue = _maxHealth;
        _healthBar.value = _maxHealth;
        _currentHealth = new NetworkVariable<float>(_maxHealth);
    }

    private void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.Q))
        {
            GameObject target = CheckPlayer();
            if (target != null)
            {
                TakeDamageServerRpc(10, target.GetComponent<NetworkObject>().OwnerClientId);
            }
        }
    }

    GameObject CheckPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (!player.GetComponent<NetworkObject>().IsOwner)
            {
                return player;
            }
        }

        return null;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    [ServerRpc]
    public void TakeDamageServerRpc(float damage, ulong targetClientId)
    {
        NetworkObject[] players = GameObject.FindObjectsByType<NetworkObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (player.OwnerClientId == targetClientId)
            {
                Debug.Log("Player " + player.OwnerClientId + " took " + damage + " damage");
                player.GetComponent<PlayerLifeManager>()._currentHealth.Value -= damage;
                player.GetComponent<PlayerLifeManager>().UpdateHealthBarClientRpc(player.GetComponent<PlayerLifeManager>()._currentHealth.Value);
            }
        }
    }

    [ClientRpc]
    void UpdateHealthBarClientRpc(float newHealth)
    {
        _healthBar.value = newHealth;
    }
}