using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerScript : NetworkBehaviour
{
    public Camera GetForeignCamera(ulong localClientId)
    {
        //if (!IsServer)
        //{
        //    Debug.Log("not a server");
        //    return null;
        //}

        if (localClientId == 0)
        {
            return GetChildByName.Get(NetworkManager.Singleton.ConnectedClients[1].PlayerObject, "Player Camera").GetComponent<Camera>();
        }

        return GetChildByName.Get(NetworkManager.Singleton.ConnectedClients[0].PlayerObject, "Player Camera").GetComponent<Camera>();
    }
}
