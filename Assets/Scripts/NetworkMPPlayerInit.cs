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
    private SkinnedMeshRenderer playerSkin;
    void Start()
    {
        if (!IsOwner)
        {
            return;
        }

        playerInput.enabled = true;
        playerCamera.enabled = true;
        playerSkin.enabled = false;
    }
}
