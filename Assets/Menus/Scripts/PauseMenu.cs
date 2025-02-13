using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject[] iconClass;
    public TMP_Text pseudo;

    public PlayerInfos playerInfos;

    private void OnEnable()
    {
        //get playerInfo
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    public void MainMenu()
    {
        //deco + open menu
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
