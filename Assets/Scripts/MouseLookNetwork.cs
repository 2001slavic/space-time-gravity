using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseLookNetwork : NetworkBehaviour
{

    public float mouseSensitivity;
    public Transform playerBody;
    private float xRotation = 0;

    void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        mouseSensitivity = PlayerPrefs.GetFloat("sensitivity", 500);
    }


    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        float mouseX = mouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        float mouseY = mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

        if (mouseX == 0 && mouseY == 0)
            return;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
