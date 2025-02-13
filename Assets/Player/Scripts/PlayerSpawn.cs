using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : NetworkBehaviour
{
    [HideInInspector] public string playerName;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Only the local player's instance should be on Ignore Raycast.
            gameObject.layer = 2; // Ignore Raycast
            GetComponentInChildren<Camera>().tag = "MainCamera";
            playerName = FindFirstObjectByType<UserSession>().dataPlayer.pseudo;
            gameObject.name = playerName;
            GetComponent<PlayerMovement>().canMove = true;
            GetComponent<PlayerUseAbilities>().canAttack = true;
            PlayerLifeManager plm = GetComponent<PlayerLifeManager>();
            Vector3 spawnPos = plm.GetRandomSpawnPosition();
            transform.position = spawnPos;
            Debug.Log(spawnPos + " et " + transform.position);
            SetPlayerPositionServerRpc(spawnPos, OwnerClientId);
        }
        else
        {
            // For remote players, set to default (layer 0) so they can be detected.
            gameObject.layer = 6;
            Debug.Log(gameObject.name + " is connected to the server");
            GetComponent<PlayerInput>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponentInChildren<DisplayCooldown>().enabled = false;
        }
    }

    [ServerRpc]
    public void SetPlayerPositionServerRpc(Vector3 position,ulong targetClientId)
    {
        PlayerLifeManager[] players = FindObjectsOfType<PlayerLifeManager>(true);
        foreach (var player in players)
        {
            if (player.OwnerClientId == targetClientId)
            {
                player.transform.position = position;
            }
        }
    }
}