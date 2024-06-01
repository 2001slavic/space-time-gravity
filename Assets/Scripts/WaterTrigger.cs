using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class WaterTrigger : MonoBehaviour
{
    [SerializeField]
    private LayerMask ignoredLayers;
    [SerializeField]
    private AudioClip enterWater;
    [SerializeField]
    private AudioClip exitWater;
    private void OnTriggerEnter(Collider other)
    {
        GameObject gameObject = other.gameObject;
        if (gameObject.layer == ignoredLayers)
        {
            return;
        }
        
        BuoyantForce buoyantForce = gameObject.GetComponent<BuoyantForce>();
        if (buoyantForce == null)
        {
            Debug.LogWarning(gameObject.name + "entered water trigger with no BuoyantForce component.");
            return;
        }

        buoyantForce.inWater = true;

        AudioSource.PlayClipAtPoint(enterWater, gameObject.transform.position, PlayerPrefs.GetFloat("volume", 0.5f));

    }

    private void OnTriggerExit(Collider other)
    {
        GameObject gameObject = other.gameObject;
        if (gameObject.layer == ignoredLayers)
        {
            return;
        }

        BuoyantForce buoyantForce = gameObject.GetComponent<BuoyantForce>();
        if (buoyantForce == null)
        {
            Debug.LogWarning(gameObject.name + "exited water trigger with no BuoyantForce component.");
            return;
        }

        buoyantForce.inWater = false;

        AudioSource.PlayClipAtPoint(exitWater, gameObject.transform.position, PlayerPrefs.GetFloat("volume", 0.5f));

    }
}
