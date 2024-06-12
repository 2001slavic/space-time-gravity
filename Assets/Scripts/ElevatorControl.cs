using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorControl : MonoBehaviour
{
    private float pressOffset;

    private Vector3 unpressedPosition;
    private Vector3 pressedPosition;

    private bool pressed;
    [SerializeField]
    private float speed;

    [SerializeField]
    private string elevatorDirection;
    [SerializeField]
    private Vector3 groundNormal;

    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioClip reverseClip;
    void Start()
    {
        pressed = false;
        pressOffset = 0.0174f;
        unpressedPosition = transform.localPosition;
        pressedPosition = transform.localPosition - groundNormal * pressOffset;
    }
    void Update()
    {
        if (pressed)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, pressedPosition, speed * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, unpressedPosition, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        pressed = true;
        if (elevatorDirection == "left")
        {
            Elevator.moveLeft = true;
        }
        else if (elevatorDirection == "right")
        {
            Elevator.moveRight = true;
        }

        AudioSource.PlayClipAtPoint(clip, other.transform.position, PlayerPrefs.GetFloat("volume", 0.5f));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        pressed = false;
        if (elevatorDirection == "left")
        {
            Elevator.moveLeft = false;
        }
        else if (elevatorDirection == "right")
        {
            Elevator.moveRight = false;
        }

        AudioSource.PlayClipAtPoint(clip, other.transform.position, PlayerPrefs.GetFloat("volume", 0.5f));
    }
}
