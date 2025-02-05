using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkLauncher : MonoBehaviour
{
    public void Start()
    {
        if (Application.platform == RuntimePlatform.LinuxServer || Application.platform == RuntimePlatform.WindowsServer)
        {
            GetComponent<NetworkManager>().StartServer();
            Debug.Log("Server started");
        }
        else
        {
            GetComponent<UnityTransport>().ConnectionData.Address = ("192.168.1.251");
            GetComponent<NetworkManager>().StartClient();
        }
    }
}
