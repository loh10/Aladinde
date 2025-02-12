using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
                    //TODO: Add respawn logic
                    
                    
                    
                    //spawn player at random location par rapport au point d'origiines.
                    
                    // remettre la vie et le shield a 0%
                    
                    //Reset des cooldowns
                    
                    //Reset des abilites
                    
                    
                    player.ResetPlayerServer(player.OwnerClientId);
                    
                    
                    
                    
                    
                    
                    
                }
            }
        }
    }

    /// <summary>
    /// Disconnects the player from the server and sends them back to the main menu
    /// </summary>
    /// <param name="player"></param>
    /// <param name="targetClientId"></param>
    private void DisconnectPlayer(PlayerLifeManager player, ulong targetClientId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { targetClientId }
            }
        };
        ChangeSceneClientRpc("MainMenu", clientRpcParams);
        player.GetComponent<NetworkObject>().Despawn();
        FindFirstObjectByType<NetworkManager>().DisconnectClient(targetClientId);
    }


    [ClientRpc]
    private void ChangeSceneClientRpc(string sceneName, ClientRpcParams clientRpcParams = default)
    {
        GetComponent<PlayerSwitchScene>().SwitchScene(sceneName);
    }


    [Rpc(SendTo.NotServer)]
    private void DisconnectPlayerRpc(ulong targetId)
    {
        if (IsOwner && OwnerClientId == targetId)
        {
            Debug.Log("Disconnecting player: " + targetId);
            GetComponent<PlayerSwitchScene>().SwitchScene("MainMenu");
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
    
    
    
    
    
    // =================== Respawn ===================
    
    void ResetPlayerServer(ulong targetClientId)
    {
        Debug.Log($"[ResetPlayerServer] called for player {targetClientId}, isServer={IsServer}");

        PlayerLifeManager[] players = FindObjectsOfType<PlayerLifeManager>(true);
        foreach (var player in players)
        {
            if (player.OwnerClientId == targetClientId)
            {
                // 1. Calcul d'une position de respawn aléatoire
                Vector3 spawnPos = GetRandomSpawnPosition(); 
                
                // 2. Déplace le joueur côté serveur
                player.transform.position = spawnPos;
                
                // 3. Réplique la position à tous les clients
                player.UpdatePositionClientRpc(spawnPos);

                // 4. Remet la santé à 100%
                player._currentHealth.Value = player._maxHealth;
                player.UpdateHealthBarClientRpc(player._currentHealth.Value);

                // 5. Remet le shield à 0
                player._currentShield.Value = 0;
                player.UpdateShieldBarClientRpc(player._currentShield.Value);

                // 6. Réinitialise les flags / effets
                player.isBurning = false;

                // 7. (Optionnel) Reset des cooldowns / abilités
                // var abilities = player.GetComponent<PlayerAbilities>();
                // if (abilities != null) abilities.ResetAllCooldowns();

                Debug.Log($"[ResetPlayerServer] Player {targetClientId} respawned at {spawnPos}. Health={player._currentHealth.Value} Shield={player._currentShield.Value}");
            }
        }
    }
    
    [ClientRpc]
    void UpdatePositionClientRpc(Vector3 spawnPos)
    {
        // Côté client, on applique la nouvelle position sur le joueur correspondant
        transform.position = spawnPos;
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        float respawnRadius = 100f;
        float randomX = Random.Range(-respawnRadius, respawnRadius);
        float randomY = Random.Range(-respawnRadius, respawnRadius);
        
        Debug.Log($"New Random spawn position: ({randomX}, {randomY})");
        
        return new Vector3(randomX, randomY, 0f);
    }
}