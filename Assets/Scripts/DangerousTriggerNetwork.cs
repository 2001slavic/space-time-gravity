using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousTriggerNetwork : MonoBehaviour
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

        PlayerDeathNetwork playerDeath = gameObject.GetComponent<PlayerDeathNetwork>();



        playerDeath.killed = true;

        if (deathClip == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(deathClip, gameObject.transform.position, PlayerPrefs.GetFloat("volume", 0.5f));
    }
}
