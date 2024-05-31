using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsSounds : MonoBehaviour
{

    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private BuoyantForce playerBuoyantForce;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] footstepsClips;

    private float curFootStepTime;
    private float footStepTime;
    void Start()
    {
        footStepTime = 0.25f;
        curFootStepTime = footStepTime;
    }

    void Update()
    {
        if (playerMovement.isGrounded && playerMovement.isRunning && !playerBuoyantForce.inWater && curFootStepTime <= 0)
        {
            audioSource.clip = footstepsClips[UnityEngine.Random.Range((int)0, footstepsClips.Length)];
            audioSource.volume = PlayerPrefs.GetFloat("volume", 0.5f) * 0.1f;
            audioSource.Play();
            curFootStepTime = footStepTime;
        }
        curFootStepTime -= Time.deltaTime;
    }
}
