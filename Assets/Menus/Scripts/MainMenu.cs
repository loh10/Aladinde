using TMPro;
using UnityEngine;
using Unity.Netcode;

public class MainMenu : MonoBehaviour
{
    public CharacterClass grillClass;
    public CharacterClass spicesClass;
    public CharacterClass herbsClass;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI grillTrophyText;
    public TextMeshProUGUI spicesTrophyText;
    public TextMeshProUGUI herbsTrophyText;
    // (Other fields as needed.)

    public void DisplayPlayerInfos(string playerName, int grillTrophy, int spicesTrophy, int herbsTrophy)
    {
        playerNameText.text = playerName;
        grillTrophyText.text = grillTrophy.ToString();
        spicesTrophyText.text = spicesTrophy.ToString();
        herbsTrophyText.text = herbsTrophy.ToString();
    }

    // Faction selection methods remain the same.
    public void SelectGrillFaction() { AssignFaction(grillClass); }
    public void SelectSpicesFaction() { AssignFaction(spicesClass); }
    public void SelectHerbsFaction() { AssignFaction(herbsClass); }

    private void AssignFaction(CharacterClass selectedClass)
    {
        // Get the local player's NetworkObject.
        var localPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (localPlayerObject != null)
        {
            // Set the player's faction.
            PlayerInfos playerInfos = localPlayerObject.GetComponent<PlayerInfos>();
            if (playerInfos != null)
            {
                playerInfos.characterClass = selectedClass;
                Debug.Log("Faction selected: " + selectedClass.className);
            }
            else
            {
                Debug.LogWarning("PlayerInfos component not found on local player.");
            }
        }
        else
        {
            Debug.LogWarning("Local player object not found.");
        }
    }
    
    // Called by the Play button.
    public void OnPlayButtonPressed()
    {
        // Get the local player's NetworkObject.
        var localPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (localPlayerObject != null)
        {
            // Get a valid spawn position using your existing logic.
            PlayerLifeManager lifeManager = localPlayerObject.GetComponent<PlayerLifeManager>();
            if (lifeManager != null)
            {
                Vector3 spawnPosition = lifeManager.GetRandomSpawnPosition();
                
                // Use the PlayerSpawn component to tell the server to teleport the player.
                PlayerSpawn playerSpawn = localPlayerObject.GetComponent<PlayerSpawn>();
                if (playerSpawn != null)
                {
                    playerSpawn.SetPlayerPositionServerRpc(spawnPosition, NetworkManager.Singleton.LocalClientId);
                }
                else
                {
                    Debug.LogWarning("PlayerSpawn not found on local player. Updating position locally (not recommended).");
                    localPlayerObject.transform.position = spawnPosition;
                }
            }
            else
            {
                Debug.LogWarning("PlayerLifeManager not found on local player.");
            }
            
            // Enable movement and attacking now.
            localPlayerObject.GetComponent<PlayerMovement>().canMove = true;
            localPlayerObject.GetComponent<PlayerUseAbilities>().canAttack = true;
        }
        else
        {
            Debug.LogWarning("Local player object not found.");
        }
        
        // Finally, hide the main menu.
        gameObject.SetActive(false);
    }
}
