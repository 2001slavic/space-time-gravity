using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerScript : NetworkBehaviour
{
    void Start()
    {
        if (!IsServer)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
    }

    void Update()
    {
        
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        if (!IsServer)
            return;
        if (obj == 1)
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<PauseNetwork>().waitForSecondPlayer = false;
        }
        else if (obj > 1)
        {
            NetworkManager.Singleton.DisconnectClient(obj);
        }
    }
}
