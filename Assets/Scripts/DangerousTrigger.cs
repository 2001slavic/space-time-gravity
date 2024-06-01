using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousTrigger : MonoBehaviour
{
    [SerializeField]
    private PlayerDeath playerDeath;
    [SerializeField]
    private TimeControl timeControl;
    [SerializeField]
    private AudioClip deathClip;
    private void OnTriggerEnter(Collider other)
    {
        if (timeControl.pauseOn || timeControl.rewindOn)
        {
            return;
        }

        playerDeath.killed = true;

        if (deathClip == null)
        {
            return;
        }


        GameObject gameObject = other.gameObject;
        AudioSource.PlayClipAtPoint(deathClip, gameObject.transform.position, PlayerPrefs.GetFloat("volume", 0.5f));
    }
}
