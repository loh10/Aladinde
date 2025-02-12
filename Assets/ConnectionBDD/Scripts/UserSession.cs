using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class UserSession : MonoBehaviour
{
    public string pseudoText; // Assigne ce champ dans l'éditeur Unity
    private string _sessionUrl;

    private const string ROOT_URL = "/auth/session.php";
    private const string DOMAIN_URL = "aladinde.ddns.net";
    private const string LOCAL_URL = "192.168.1.251";
    
    
    void Start()
    {
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
}