using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GetAllPlayerInLobby : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerInLobbyTxt;
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private GameObject _botPrefab;
    [SerializeField] private int _nbMaxPlayer = 3;
    [SerializeField] private float _waitingTimeBeforeAddBot = 20f;

    public override void OnNetworkSpawn()
    {
        // S'exécute uniquement côté serveur
        if (IsServer)
        {
            Debug.Log("Server");
            StartCoroutine(CheckPlayerInLobby());
        }
    }

    IEnumerator CheckPlayerInLobby()
    {
        float elapsedTime = 0f;
        while (_networkManager.ConnectedClientsList.Count < _nbMaxPlayer && elapsedTime < _waitingTimeBeforeAddBot)
        {
            UpdatePlayerInRoomClientRpc(_networkManager.ConnectedClientsList.Count);
            yield return new WaitForSeconds(1f);
            elapsedTime++;
        }
        UpdatePlayerInRoomClientRpc(_networkManager.ConnectedClientsList.Count, true);
        SpawnBot();
    }

    [ClientRpc]
    private void UpdatePlayerInRoomClientRpc(int playerInRoom, bool useBot = false)
    {
        _playerInLobbyTxt.text = playerInRoom + " / " + _nbMaxPlayer;
        if (playerInRoom >= _nbMaxPlayer || useBot)
        {
            if (NetworkManager.Singleton.LocalClient != null &&
                NetworkManager.Singleton.LocalClient.PlayerObject != null)
            {
                PlayerMovement playerMovement = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.canMove = true;
                }
            }
            gameObject.SetActive(false);
        }
    }

    public void SpawnBot()
    {
        for (int i = 0; i < _nbMaxPlayer - _networkManager.ConnectedClientsList.Count; i++)
        {
            GameObject bot = Instantiate(_botPrefab);
            NetworkObject networkObject = bot.GetComponent<NetworkObject>();
            networkObject.Spawn();
        }
    }
}
