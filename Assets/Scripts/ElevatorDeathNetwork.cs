using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDeathNetwork : MonoBehaviour
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
        SizeControlNetwork sizeControl = gameObject.GetComponent<SizeControlNetwork>();
        PlayerDeathNetwork playerDeath = gameObject.GetComponent<PlayerDeathNetwork>();
        PlayerMovementNetwork playerMovement = gameObject.GetComponent<PlayerMovementNetwork>();

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
}
