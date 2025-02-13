using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Serialization;

public class UserSession : MonoBehaviour
{
    public string pseudoText; // Assigne ce champ dans l'éditeur Unity
    private string _sessionUrl;

    private const string ROOT_URL = "/auth/session.php";
    private const string DOMAIN_URL = "aladinde.ddns.net";
    private const string LOCAL_URL = "192.168.1.251";
    
    
    public TMP_Text TMP_pseudoText;
    public TMP_Text TMP_emailText;
    public TMP_Text TMP_trophy_grillText;
    public TMP_Text TMP_trophy_spiceText;
    public TMP_Text TMP_trophy_herbText;
    
    void Start()
    {
        string webGLUrl = Application.absoluteURL; // URL PAGE 
        Debug.Log("WebGL URL: " + webGLUrl);

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
            pseudoText = "Erreur : URL non reconnue";
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
                pseudoText = "Erreur de connexion";
            }
            else
            {
                string json = request.downloadHandler.text;
                //Debug.Log($"Réponse JSON reçue : {json}");

                UserResponse response = JsonUtility.FromJson<UserResponse>(json);
                
                if (response != null && !string.IsNullOrEmpty(response.pseudo))
                {
                    pseudoText = response.pseudo;
                    
                    Debug.Log("Pseudo : " + response.pseudo);
                    Debug.Log("Email : " + response.email);
                    Debug.Log("Trophée Grill : " + response.trophy_grill);
                    Debug.Log("Trophée Spice : " + response.trophy_spice);
                    Debug.Log("Trophée Herb : " + response.trophy_herb);
                    
                    
                    TMP_pseudoText.text = response.pseudo;
                    TMP_emailText.text = response.email;
                    TMP_trophy_grillText.text = response.trophy_grill.ToString();
                    TMP_trophy_spiceText.text = response.trophy_spice.ToString();
                    TMP_trophy_herbText.text = response.trophy_herb.ToString();
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
}