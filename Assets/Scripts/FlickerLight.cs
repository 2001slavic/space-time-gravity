using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{

    public Light light;
    public Material enabledMaterial;
    public Material disabledMaterial;

    public AudioSource audioSource;
    public AudioClip[] audioClips;
    void Start()
    {
        light = GetComponent<Light>();
        audioSource.volume = PlayerPrefs.GetFloat("volume", 0.5f);
    }

    void Update()
    {
        if (Random.Range((int)0, (int)100) == 0)
        {
            light.enabled = !light.enabled;
            if (light.enabled)
            {
                gameObject.GetComponent<MeshRenderer>().material = enabledMaterial;
                audioSource.clip = audioClips[Random.Range((int)0, audioClips.Length)];
                audioSource.Play();
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material = disabledMaterial;
            }
        }
    }
}
