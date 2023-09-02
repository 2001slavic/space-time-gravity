using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickLook : MonoBehaviour
{   
    public float joyLookSensitivity = 500;
    public Transform playerBody;
    private float xRotation = 0;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        joyLookSensitivity = PlayerPrefs.GetFloat("jsensitivity", 500);
    }

    void Update()
    {
        float x = joyLookSensitivity * Input.GetAxis("RHorizontal") * Time.deltaTime;
        float y = joyLookSensitivity * Input.GetAxis("RVertical") * Time.deltaTime;

        if (x == 0 && y == 0)
            return;

        xRotation -= y;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * x);
    }
}
