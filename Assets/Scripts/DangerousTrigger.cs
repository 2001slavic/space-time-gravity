using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioClip deathClip;
    [SerializeField]
    private bool ignoreTimeControl;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        GameObject gameObject = other.gameObject;

        TimeControl timeControl = gameObject.GetComponent<TimeControl>();
        PlayerDeath playerDeath = gameObject.GetComponent<PlayerDeath>();


        if (timeControl != null && !ignoreTimeControl && (timeControl.pauseOn || timeControl.rewindOn))
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
