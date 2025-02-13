using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TrophyUpdater : MonoBehaviour
{
    private string _sessionUrl;
    
    private const string ROOT_URL = "/auth/update_trophy.php";
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
            return;
        }
    }
    
    /// <summary>
    /// Met à jour un trophée (ou tout autre champ) pour un utilisateur.
    /// </summary>
    /// <param name="pseudo">Le pseudo de l'utilisateur</param>
    /// <param name="trophyField">Le nom du champ à mettre à jour (par ex. "trophy_grill" ou "index_hat")</param>
    /// <param name="value">La valeur à ajouter</param>
    public IEnumerator UpdateTrophy(string pseudo, string trophyField, int value)
    {
        Debug.Log("Pseudo récupéré et passee : " + pseudo);
        WWWForm form = new WWWForm();
        form.AddField("pseudo", pseudo);
        form.AddField("trophy", trophyField);
        form.AddField("value", value);

        UnityWebRequest www = UnityWebRequest.Post(_sessionUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || 
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Erreur lors de la mise à jour du trophée : " + www.error);
        }
        else
        {
            Debug.Log("Réponse du serveur : " + www.downloadHandler.text);
        }
        
        Debug.Log("Nombre de trophy : " + value);
    }
}