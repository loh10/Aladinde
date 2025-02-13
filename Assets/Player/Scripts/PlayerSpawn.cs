using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : NetworkBehaviour
{
    [HideInInspector] public string playerName;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // When the player spawns, immediately move them off–map on the server.
            Vector3 offMapPos = new Vector3(10000, 10000, 0);
            transform.position = offMapPos;
            // (This ClientRpc forces clients to update the position—if your NetworkTransform is configured properly this may be redundant.)
            UpdatePositionClientRpc(offMapPos);
        }
    
        if (IsOwner)
        {
            // Set up local player properties.
            gameObject.layer = 2; // For example, ignore raycasts.
            GetComponentInChildren<Camera>().tag = "MainCamera";
            // Assumes UserSession holds dataPlayer with the pseudo.
            playerName = FindObjectOfType<UserSession>().dataPlayer.pseudo;
            gameObject.name = playerName;
        }
        else
        {
            // For remote players.
            gameObject.layer = 6;
            GetComponent<PlayerInput>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponentInChildren<DisplayCooldown>().enabled = false;
        }
    }

    // This ClientRPC is defined with ClientRpcParams so we can target specific clients.
    [ClientRpc]
    public void UpdatePositionClientRpc(Vector3 position, ClientRpcParams clientRpcParams = default)
    {
        transform.position = position;
    }
    
    // This ServerRPC sets the position on the server for the player with the given clientId.
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerPositionServerRpc(Vector3 position, ulong targetClientId)
    {
        // Get the player's NetworkObject using the SpawnManager.
        NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(targetClientId);
        if (playerObject != null)
        {
            // Set the server's transform.
            playerObject.transform.position = position;
            // Then, send a ClientRpc update only to that client.
            UpdatePositionClientRpc(position, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { targetClientId }
                }
            });
        }
        else
        {
            Debug.LogWarning("Player object not found for targetClientId: " + targetClientId);
        }
    }
}
