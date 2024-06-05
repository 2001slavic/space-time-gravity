using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtonPress : MonoBehaviour
{
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private float handLength;
    [SerializeField]
    private LayerMask ignoredLayers;

    private void OnUse()
    {
        RaycastHit[] handHit = Physics.RaycastAll(playerCamera.transform.position, playerCamera.transform.forward, handLength, ~ignoredLayers);
        float minDistance = Mathf.Infinity;
        int minIndex = -1;
        for (int i = 0; i < handHit.Length; i++)
        {
            if (handHit[i].distance < minDistance)
            {
                minDistance = handHit[i].distance;
                minIndex = i;
            }
            else if (handHit[i].distance == minDistance && handHit[i].collider.CompareTag("Button"))
            {
                minDistance = handHit[i].distance;
                minIndex = i;
            }
        }
        if (minIndex != -1)
        {
            Debug.Log(handHit[minIndex].collider.gameObject.name);
        }
        if (minIndex != -1 && handHit[minIndex].collider.CompareTag("Button"))
        {
            handHit[minIndex].collider.gameObject.GetComponent<GameButton>().pressed = true;
        }
    }
}
