using Unity.Netcode;
using UnityEngine;



public class EndGameMenu : MonoBehaviour
{
    private PlayerLifeManager _playerLifeManager;
    private NetworkManager _playerID;

    private void Start()
    {
        _playerLifeManager = FindAnyObjectByType<PlayerLifeManager>();
        _playerID = FindAnyObjectByType<NetworkManager>();
    }

    public void BackToMainMenu()
    {
        if (_playerLifeManager != null && _playerID != null && _playerID.IsClient)
        {
            _playerLifeManager.DisconnectPlayer(_playerLifeManager, _playerID.LocalClientId);
        }
    }
}
