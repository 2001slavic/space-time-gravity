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
    private GameObject eventSystem;

    private void PreventDrawingSelf(GameObject gameObject, string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
        playerCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(layerName));
    }

    void Start()
    {
        if (!IsLocalPlayer)
        {
            if (NetworkManager.Singleton.LocalClientId == 0)
            {
                PreventDrawingSelf(gameObject, "Player2");
            }
            else
            {
                PreventDrawingSelf(gameObject, "Player1");
            }
            NetworkObject localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
            PictureInPicture pictureInPicture = localPlayer.GetComponent<PictureInPicture>();
            pictureInPicture.foreignCamera = playerCamera;
            pictureInPicture.foreignCamera.rect = new Rect(0.0625f, 0.0625f, 0.25f, 0.25f);
            return;
        }

        if (!IsOwner)
        {
            return;
        }

        playerInput.enabled = true;
        playerCamera.enabled = true;
        audioListener.enabled = true;
        eventSystem.SetActive(true);

        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            PreventDrawingSelf(gameObject, "Player1");
        }
        else
        {
            PreventDrawingSelf(gameObject, "Player2");
        }    
    }
}
