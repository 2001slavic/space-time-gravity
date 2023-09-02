using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{
    public bool pressed;

    public Material pressedMaterial;
    public OpenDoor[] openDoors;
    public GameObject button;

    private bool lastPressed;
    void Start()
    {
        lastPressed = false;
        pressed = false;
    }

    void Update()
    {
        if (pressed != lastPressed)
        {
            lastPressed = true;
            button.GetComponent<MeshRenderer>().material = pressedMaterial;

            foreach (OpenDoor openDoor in openDoors)
            {
                openDoor.isOpen = true;
            }
        }
    }
}
