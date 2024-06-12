using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDeath : MonoBehaviour
{
    [SerializeField]
    private AudioClip deathClip;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        GameObject gameObject = other.gameObject;
        SizeControl sizeControl = gameObject.GetComponent<SizeControl>();
        PlayerDeath playerDeath = gameObject.GetComponent<PlayerDeath>();
        PlayerMovement playerMovement = gameObject.GetComponent<PlayerMovement>();

        if (!playerMovement.isGrounded)
        {
            return;
        }

        if (sizeControl.ChangeToLower())
        {
            return;
        }
        playerDeath.killed = true;
        

        if (deathClip == null)
        {
            return;
        }
        AudioSource.PlayClipAtPoint(deathClip, gameObject.transform.position, PlayerPrefs.GetFloat("volume", 0.5f));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
    }
}
