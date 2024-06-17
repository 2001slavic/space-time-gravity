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

    [SerializeField]
    private GameObject toDisable;

    [SerializeField]
    private int minimalKeys;

    private int currentKeys;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("ObjectiveKey"))
        {
            return;
        }

        currentKeys++;

        if (currentKeys < minimalKeys)
        {
            return;
        }

        gameObject.GetComponent<MeshRenderer>().material = activeMaterial;
        gameObject.GetComponent<Light>().color = activeColor;

        if (toDisable != null)
        {
            toDisable.SetActive(false);
            return;
        }
        openDoor.isOpen = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.collider.CompareTag("ObjectiveKey"))
        {
            return;
        }

        currentKeys--;
        if (alwaysActive)
            return;
        gameObject.GetComponent<MeshRenderer>().material = inactiveMaterial;
        gameObject.GetComponent<Light>().color = inactiveColor;
        if (toDisable != null)
        {
            toDisable.SetActive(true);
            return;
        }
        openDoor.isOpen = false;
    }
    void Start()
    {
        currentKeys = 0;
        gameObject.GetComponent<MeshRenderer>().material = inactiveMaterial;
        gameObject.GetComponent<Light>().color = inactiveColor;
        if (toDisable == null)
        {
            openDoor.isOpen = false;
        }
    }

    void Update()
    {
        
    }
}
