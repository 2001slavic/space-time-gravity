using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorTrigger : MonoBehaviour
{

    public OpenDoor openDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            openDoor.isOpen = true;
        }
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
