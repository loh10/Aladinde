using Unity.Netcode;
using UnityEngine;

public class NetworkLauncher : MonoBehaviour
{
    public void Start()
    {
        if (Application.platform == RuntimePlatform.LinuxServer)
        {
            GetComponent<NetworkManager>().StartServer();
            Debug.Log("Server started");
        }
        else
        {
            GetComponent<NetworkManager>().StartClient();
        }
    }
}
