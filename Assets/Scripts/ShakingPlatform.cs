using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakingPlatform : MonoBehaviour
{

    [SerializeField]
    private Vector3 originalPosition;
    [SerializeField]
    private Vector3 fallenPosition;

    [SerializeField]
    private TimeControl timeControl;

    [HideInInspector]
    public int currentState;
    [HideInInspector]
    public float shakeTimer;

    [SerializeField]
    private float shakeSpeed;
    [SerializeField]
    private float shakeAngle;
    [SerializeField]
    private GameObject centerPlatform;

    private Vector3 colliderCenter;

    private Rewindable rewindable;

    private Rigidbody playerRb;

    private readonly float waitFallTimeout = 2f;
    private readonly float platformFallSpeed = 30f;

    private readonly Dictionary<int, string> states = new()
    {
        { 1, "PLAYER_NOT_ENTERED_TRIGGER" },
        { 2, "PLATFORM_SHAKING"           },
        { 3, "PLATFORM_FALLING"           },
        { 4, "PLATFORM_FELL"              }
    };

    private int GetNextState(bool transitionCondition)
    {
        return transitionCondition ? currentState + 1 : currentState;
    }

    private void Shake()
    {
        float zRotation = transform.parent.localEulerAngles.z;
        if (zRotation > 180)
            zRotation -= 360;
        if (zRotation < -shakeAngle)
        {
            shakeSpeed = Mathf.Abs(shakeSpeed);
        }
        else if (zRotation >= shakeAngle)
        {
            shakeSpeed = -Mathf.Abs(shakeSpeed);
        }
        transform.parent.RotateAround(colliderCenter, new Vector3(0, 0, 1), shakeSpeed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerRb = other.GetComponent<Rigidbody>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerRb = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (timeControl.rewindOn)
        {
            return;
        }

        if (states[currentState] == "PLAYER_NOT_ENTERED_TRIGGER")
        {
            currentState = GetNextState(true); // next state: "PLATFORM_SHAKING" 
        }
        else if (states[currentState] == "PLATFORM_FALLING")
        {
            currentState = GetNextState(transform.parent.localPosition == fallenPosition); // next state: "PLATFORM_FELL" 
        }
    }

    private void Awake()
    {
        shakeTimer = waitFallTimeout;
        currentState = 1;
        colliderCenter = centerPlatform.GetComponent<Collider>().bounds.center;
        rewindable = gameObject.GetComponentInParent<Rewindable>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (playerRb != null && timeControl.rewindOn && states[currentState] == "PLATFORM_FALLING")
        {
            Vector3 positionDifference = rewindable.PeekPosition() - transform.parent.position;
            playerRb.position += positionDifference.normalized * platformFallSpeed * Time.deltaTime;
            if (states[rewindable.PeekShakingPlatformState()] == "PLATFORM_SHAKING")
            {
                Vector3 forceToApply = playerRb.transform.up * platformFallSpeed;
                playerRb.velocity += forceToApply;
            }
        }

        if (states[currentState] == "PLATFORM_SHAKING")
        {
            Shake();
            shakeTimer -= Time.deltaTime;
            currentState = GetNextState(shakeTimer <= 0); // next state: "PLATFORM_FALLING" 
        }
        else if (states[currentState] == "PLATFORM_FALLING")
        {
            transform.parent.localRotation = Quaternion.identity;
            transform.parent.localPosition = Vector3.MoveTowards(transform.parent.localPosition, fallenPosition, platformFallSpeed * Time.deltaTime);
        }
    }
}
