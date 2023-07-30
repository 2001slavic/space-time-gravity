using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public float mouseSensitivity = 500;
    public Transform playerBody;
    public PlayerMovement playerMovement;
    private float xRotation = 0;
    private Vector3 lastGroundNormal;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        lastGroundNormal = playerMovement.groundNormal;
    }


    void Update()
    {
        float mouseX = mouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        float mouseY = mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);

        if (playerMovement.groundNormal != lastGroundNormal)
        {
            playerBody.rotation = Quaternion.FromToRotation(playerBody.up, playerMovement.groundNormal) * playerBody.rotation;
            lastGroundNormal = playerMovement.groundNormal;
        }
        
    }
}
