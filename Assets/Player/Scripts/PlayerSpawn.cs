using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : NetworkBehaviour
{
    [HideInInspector]
    public string playerName;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Only the local player's instance should be on Ignore Raycast.
            gameObject.layer = 2; // Ignore Raycast
            GetComponentInChildren<Camera>().tag = "MainCamera";
            playerName = FindFirstObjectByType<UserSession>().pseudoText;
            gameObject.name = playerName;
            GetAllPlayerInLobby playerLobby = FindFirstObjectByType<GetAllPlayerInLobby>();
            Debug.Log(playerLobby._gameState.currentState);
            if (playerLobby._gameState.currentState == GameStateEnum.InGame)
            {
                GetComponent<PlayerMovement>().canMove = true;
                GetComponent<PlayerUseAbilities>().canAttack = true;
                playerLobby.gameObject.SetActive(false);
            }
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
}