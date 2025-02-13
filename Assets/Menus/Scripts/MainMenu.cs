using UnityEngine;
using Unity.Netcode;

public class MainMenu : MonoBehaviour
{
    public CharacterClass grillClass;
    public CharacterClass spicesClass;
    public CharacterClass herbsClass;
    
    public void HideMenu()
    {
        gameObject.SetActive(false);
    }
    
    // Called by the button for the Grill faction.
    public void SelectGrillFaction()
    {
        AssignFaction(grillClass);
    }
    
    // Called by the button for the Épices faction.
    public void SelectSpicesFaction()
    {
        AssignFaction(spicesClass);
    }
    
    // Called by the button for the Herbes faction.
    public void SelectHerbsFaction()
    {
        AssignFaction(herbsClass);
    }

    private void AssignFaction(CharacterClass selectedClass)
    {
        // Get the local player's NetworkObject.
        var localPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (localPlayerObject != null)
        {
            // Get the PlayerInfos component.
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
        
        // Optionally hide the main menu once a selection is made.
        gameObject.SetActive(false);
    }
}