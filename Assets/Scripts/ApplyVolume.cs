using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyVolume : MonoBehaviour
{
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        audioSource.volume = PlayerPrefs.GetFloat("volume", 0.5f);
    }
}
