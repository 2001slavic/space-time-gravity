using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveLock : MonoBehaviour
{
    public bool alwaysActive;
    public Material activeMaterial;
    public Material inactiveMaterial;
    public Color activeColor;
    public Color inactiveColor;

    public OpenDoor openDoor;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("ObjectiveKey"))
        {
            gameObject.GetComponent<MeshRenderer>().material = activeMaterial;
            gameObject.GetComponent<Light>().color = activeColor;
            openDoor.isOpen = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("ObjectiveKey"))
        {
            if (alwaysActive)
                return;
            gameObject.GetComponent<MeshRenderer>().material = inactiveMaterial;
            gameObject.GetComponent<Light>().color = inactiveColor;
            openDoor.isOpen = false;
        }
    }
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().material = inactiveMaterial;
        gameObject.GetComponent<Light>().color = inactiveColor;
        openDoor.isOpen = false;
    }

    void Update()
    {
        
    }
}
