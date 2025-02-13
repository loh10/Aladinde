using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject[] iconClass;
    public TMP_Text pseudo;

    public PlayerInfos playerInfos;
    private NetworkManager _networkManager;

    private void OnEnable()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
        playerInfos = _networkManager.LocalClient.PlayerObject.GetComponent<PlayerInfos>();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    public void MainMenu()
    {
        //deco + open menu
        _networkManager.LocalClient.PlayerObject.GetComponent<PlayerLifeManager>().DisconnectPlayer(_networkManager.LocalClient.PlayerObject.GetComponent<PlayerLifeManager>(),_networkManager.LocalClient.PlayerObject.OwnerClientId);
    }

    public void GetPlayerInfo()
    {
        foreach (GameObject icon in iconClass)
        {
            icon.SetActive(false);
        }

        int classIndex = System.Array.IndexOf(playerInfos._classes, playerInfos.characterClass);

        switch (classIndex)
        {
            case 0:
                iconClass[0].SetActive(true);
                break;
            case 1:
                iconClass[1].SetActive(true);
                break;
            case 2:
                iconClass[2].SetActive(true);
                break;
            default:
                break;
        }
    }
}
