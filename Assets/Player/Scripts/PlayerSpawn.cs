using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : NetworkBehaviour
{
    [HideInInspector]public string playerName;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Debug.Log(gameObject.name + " is connected to the server");
            GetComponent<PlayerInput>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponent<SpriteRenderer>().color = Color.red;
            return;
        }
        GetComponentInChildren<Camera>().tag = "MainCamera";
        playerName = FindFirstObjectByType<UserSession>().pseudoText;
        gameObject.name = playerName;
    }
}
