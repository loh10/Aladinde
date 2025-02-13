using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UserSession : MonoBehaviour
{
    private string _sessionUrl;

    private const string ROOT_URL = "/auth/session.php";
    private const string DOMAIN_URL = "aladinde.ddns.net";
    private const string LOCAL_URL = "192.168.1.251";

    [Header("Data")]
    public UserResponse dataPlayer;
    private TrophyUpdater trophyUpdater;
    
    void Start()
    {
        trophyUpdater = GetComponent<TrophyUpdater>();
        
        string webGLUrl = Application.absoluteURL; // URL PAGE 
        //Debug.Log("WebGL URL: " + webGLUrl);

        // Déterminer l'URL racine
        if (webGLUrl.Contains($"{DOMAIN_URL}"))
        {
            _sessionUrl = $"{DOMAIN_URL}{ROOT_URL}";
        }
        else if (webGLUrl.Contains($"{LOCAL_URL}"))
        {
            _sessionUrl = $"{LOCAL_URL}{ROOT_URL}";
        }
        else
        {
            Debug.LogError("URL inconnue, session non définie.");
            dataPlayer.pseudo = "Erreur : URL non reconnue";
            return;
        }

        StartCoroutine(GetUserPseudo());
    }

    IEnumerator GetUserPseudo()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(_sessionUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Erreur de connexion : " + request.error);
                dataPlayer.pseudo = "Erreur de connexion";
            }
            else
            {
                string json = request.downloadHandler.text;
                //Debug.Log($"Réponse JSON reçue : {json}");

                UserResponse response = JsonUtility.FromJson<UserResponse>(json);
                
                if (response != null && !string.IsNullOrEmpty(response.pseudo))
                {
                    Debug.Log("Pseudo : " + response.pseudo);
                    Debug.Log("Email : " + response.email);
                    Debug.Log("Trophée Grill : " + response.trophy_grill);
                    Debug.Log("Trophée Spice : " + response.trophy_spice);
                    Debug.Log("Trophée Herb : " + response.trophy_herb);
                    Debug.Log("Index Hat : " + response.index_hat);
                    
                    dataPlayer.pseudo= response.pseudo;
                    dataPlayer.email = response.email;
                    dataPlayer.trophy_grill = response.trophy_grill;
                    dataPlayer.trophy_spice = response.trophy_spice;
                    dataPlayer.trophy_herb = response.trophy_herb;
                    dataPlayer.index_hat = response.index_hat;
                    
                    trophyUpdater.StartCoroutine(trophyUpdater.UpdateTrophy(dataPlayer.pseudo, Trophy.trophy_grill.ToString(), Random.Range(1, 10)));
                }
                else
                {
                    Debug.Log("Utilisateur non connecté, redirection...");
                    Application.OpenURL(response.redirect);
                }
            }
        }
    }
}

[System.Serializable]
public class UserResponse
{
    public string pseudo;
    public string redirect;
    public string email;
    public int trophy_grill;
    public int trophy_spice;
    public int trophy_herb;
    public int index_hat;
}

[System.Serializable]
public enum Trophy
{
    trophy_grill,
    trophy_spice,
    trophy_herb,
    index_hat
}
