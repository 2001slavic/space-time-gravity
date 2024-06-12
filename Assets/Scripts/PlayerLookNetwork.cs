using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookNetwork : NetworkBehaviour
{
    [SerializeField]
    private Transform playerBody;
    private float xRotation;
    [HideInInspector]
    public float x;
    [HideInInspector]
    public float y;

    [SerializeField]
    private PlayerInput playerInput;

    void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        xRotation = 0;
    }

    private void OnLook(InputValue value)
    {
        if (!IsOwner)
        {
            return;
        }
        Vector2 vector2value = value.Get<Vector2>();
        x = vector2value.x;
        y = vector2value.y;
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        float sensitivity = PlayerPrefs.GetFloat("sensitivity" + playerInput.playerIndex, 500);

        xRotation -= y * sensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * x * sensitivity * Time.deltaTime);
    }
}
