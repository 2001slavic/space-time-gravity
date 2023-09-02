using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public bool isOpen;
    private bool lastOpen;

    public Transform leftDoorTransform;
    public Transform rightDoorTransform;

    public Vector3 leftDoorOpenDirection;
    public float moveWith = 1.28048f;

    public float delta = 0.01f;

    public AudioSource audioSource;
    public AudioClip audioClip;

    private Vector3 closedLeftDoorPosition;
    private Vector3 closedRightDoorPosition;
    private Vector3 openLeftDoorPosition;
    private Vector3 openRightDoorPosition;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        isOpen = false;
        lastOpen = false;
        closedLeftDoorPosition = leftDoorTransform.position;
        closedRightDoorPosition = rightDoorTransform.position;
        openLeftDoorPosition = closedLeftDoorPosition + leftDoorOpenDirection * moveWith;
        openRightDoorPosition = closedRightDoorPosition - leftDoorOpenDirection * moveWith;
    }

    void Update()
    {
        float diff;
        if (isOpen)
        {
            if (lastOpen != isOpen)
            {
                audioSource.clip = audioClip;
                audioSource.volume = PlayerPrefs.GetFloat("volume", 0.5f);
                audioSource.Play();
            }
            diff = (leftDoorTransform.position - openLeftDoorPosition).magnitude;
            if (diff >= delta)
                leftDoorTransform.position += leftDoorOpenDirection * Time.deltaTime;

            diff = (rightDoorTransform.position - openRightDoorPosition).magnitude;
            if (diff >= delta)
                rightDoorTransform.position -= leftDoorOpenDirection * Time.deltaTime;
        }
        else
        {
            diff = (leftDoorTransform.position - closedLeftDoorPosition).magnitude;
            if (diff >= delta)
                leftDoorTransform.position -= leftDoorOpenDirection * Time.deltaTime;

            diff = (rightDoorTransform.position - closedRightDoorPosition).magnitude;
            if (diff >= delta)
                rightDoorTransform.position += leftDoorOpenDirection * Time.deltaTime;
        }
        lastOpen = isOpen;
    }
}
