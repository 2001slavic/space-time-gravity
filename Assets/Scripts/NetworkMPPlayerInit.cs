using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkMPPlayerInit : NetworkBehaviour
{

    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private AudioListener audioListener;
    [SerializeField]
    private SkinnedMeshRenderer playerSkin;
    [SerializeField]
    private GameObject eventSystem;
    void Start()
    {

        //NetworkManager.ConnectionApprovalCallback = ConnectionApprovalCallback;
        if (!IsOwner)
        {
            return;
        }

        playerInput.enabled = true;
        playerCamera.enabled = true;
        audioListener.enabled = true;
        playerSkin.enabled = false;
        eventSystem.SetActive(true);
    }

    private Vector3 GetPlayerSpawnPosition()
    {
        return Vector3.zero;
    }

    //private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    //{
    //    response.Approved = true;
    //    response.CreatePlayerObject = true;
    //    response.Position = GetPlayerSpawnPosition();
    //    Debug.Log(NetworkManager.Singleton.LocalClientId);
    //}
}
