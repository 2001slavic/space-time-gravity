using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    private Transform playerBody;
    private float xRotation;
    [HideInInspector]
    public float x;
    [HideInInspector]
    public float y;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        xRotation = 0;
    }

    private void OnLook(InputValue value)
    {
        Vector2 vector2value = value.Get<Vector2>();
        x = vector2value.x;
        y = vector2value.y;
    }

    void Update()
    {
        //float mouseX = mouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        //float mouseY = mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

        //if (mouseX == 0 && mouseY == 0)
        //    return;

        //xRotation -= mouseY;
        //xRotation = Mathf.Clamp(xRotation, -90, 90);

        //transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //playerBody.Rotate(Vector3.up * mouseX);

        float sensitivity = PlayerPrefs.GetFloat("sensitivity", 500);

        xRotation -= y * sensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * x * sensitivity * Time.deltaTime);
    }
}
