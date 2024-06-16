using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PictureInPicture : NetworkBehaviour
{
    public Camera foreignCamera;
    [SerializeField]
    private PlayerInput playerInput;

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (foreignCamera == null)
        {
            return;
        }
        if (playerInput.actions["PictureInPicture"].ReadValue<float>() != 1)
        {
            foreignCamera.enabled = false;
            return;
        }

        foreignCamera.enabled = true;
    }
}
