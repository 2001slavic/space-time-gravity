using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DisconnectThirdPlayer : NetworkBehaviour
{

    void Start()
    {
    }

    void Update()
    {
        if (!IsServer)
            return;
        Debug.Log("isServer");
        bool oldState = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<PauseNetwork>().waitForSecondPlayer;
        if (oldState && NetworkManager.Singleton.ConnectedClients.Count >= 1)
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<PauseNetwork>().waitForSecondPlayer = false;
        }
    }
}
