using System.Collections;
using TMPro;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using UnityEngine;

public class GetAllPlayerInLobby : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerInLobbyTxt;
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private GameObject _botPrefab;
    [SerializeField] private int _nbMaxPlayer = 3;
    [SerializeField] private float _waitingTimeBeforeAddBot = 20f;
    private float elapsedTime = 0;
    public GameState _gameState { get; private set; } = new GameState();

    public override void OnNetworkSpawn()
    {
        // S'exécute uniquement côté serveur


        if (IsServer)
        {
            if (_networkManager.ConnectedClientsList.Count > 1)
            {
                _gameState.currentState = GameStateEnum.InGame;
            }
            Debug.Log("Server");
            if (_networkManager != null)
            {
                StartCoroutine(CheckPlayerInLobby());
            }
            else
            {
                Debug.LogError("NetworkManager is not assigned.");
            }
        }
    }

    IEnumerator CheckPlayerInLobby()
    {
        elapsedTime = 0;
        while (_networkManager.ConnectedClientsList.Count < _nbMaxPlayer && elapsedTime < _waitingTimeBeforeAddBot)
        {
            UpdatePlayerInRoomClientRpc(_networkManager.ConnectedClientsList.Count);
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        UpdatePlayerInRoomClientRpc(_networkManager.ConnectedClientsList.Count, true);
        SpawnBot();
    }

    [ClientRpc]
    private void UpdatePlayerInRoomClientRpc(int playerInRoom, bool useBot = false)
    {
        if (_playerInLobbyTxt != null)
        {
            _playerInLobbyTxt.text = playerInRoom + " / " + _nbMaxPlayer;
        }
        else
        {
            Debug.LogError("PlayerInLobbyTxt is not assigned.");
        }

        if (playerInRoom >= _nbMaxPlayer || useBot)
        {
            if (_networkManager.LocalClient != null &&
                _networkManager.LocalClient.PlayerObject != null)
            {
                PlayerMovement playerMovement = _networkManager.LocalClient.PlayerObject.GetComponent<PlayerMovement>();
                PlayerUseAbilities playerUseAbilities = _networkManager.LocalClient.PlayerObject.GetComponent<PlayerUseAbilities>();
                if (playerMovement != null && playerUseAbilities != null)
                {
                    playerMovement.canMove = true;
                    playerUseAbilities.canAttack = true;
                }
            }
            _gameState.currentState = GameStateEnum.InGame;
            gameObject.SetActive(false);
        }
    }

    public void SpawnBot()
    {
        if (_botPrefab == null)
        {
            Debug.LogError("BotPrefab is not assigned.");
            return;
        }

        for (int i = 0; i < _nbMaxPlayer - _networkManager.ConnectedClientsList.Count; i++)
        {
            GameObject bot = Instantiate(_botPrefab);
            NetworkObject networkObject = bot.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn();
            }
            else
            {
                Debug.LogError("Bot prefab does not have a NetworkObject component.");
            }
        }
    }
}