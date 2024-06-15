using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class Rewindable : MonoBehaviour
{
    public TimeControl timeControl;
    public Rigidbody rb;
    public Light light;
    [SerializeField]
    private ShakingPlatform shakingPlatform;

    public static int HISTORY_STACK_SIZE;
    private EnumerableDropOutStack<Vector3> positionHistory;
    private EnumerableDropOutStack<Quaternion> rotationHistory;
    private EnumerableDropOutStack<Vector3> rbVelocityHistory;
    private EnumerableDropOutStack<Material> materialHistory;
    private EnumerableDropOutStack<bool> lightStateHistory;
    private EnumerableDropOutStack<int> shakingPlatformStateHistory;
    private EnumerableDropOutStack<float> shakingPlatformShakeTimerHistory;
    private Vector3 freezePos;
    private Quaternion freezeRotation;
    private Vector3 freezeRbVelocity;
    private Material freezeMaterial;
    private bool freezeLightState;
    private int freezeShakingPlatformState;
    private float freezeShakingPlatformShakeTimer;
    public bool waitingForPause;
    void Start()
    {
        HISTORY_STACK_SIZE = Mathf.RoundToInt(2000 * timeControl.effectMaxTime);
        rb = GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
            rbVelocityHistory = new EnumerableDropOutStack<Vector3>(HISTORY_STACK_SIZE);
        light = GetComponent<Light>();
        if (light != null)
            lightStateHistory = new EnumerableDropOutStack<bool>(HISTORY_STACK_SIZE);
        if (shakingPlatform != null)
        {
            shakingPlatformStateHistory = new EnumerableDropOutStack<int>(HISTORY_STACK_SIZE);
            shakingPlatformShakeTimerHistory = new EnumerableDropOutStack<float>(HISTORY_STACK_SIZE);
        }
            

        positionHistory = new EnumerableDropOutStack<Vector3>(HISTORY_STACK_SIZE);
        rotationHistory = new EnumerableDropOutStack<Quaternion>(HISTORY_STACK_SIZE);
        if (gameObject.GetComponent<Renderer>() != null)
            materialHistory = new EnumerableDropOutStack<Material>(HISTORY_STACK_SIZE);
        waitingForPause = true;
        freezePos = transform.position;
    }

    void Update()
    {
        if (timeControl.pauseOn)
        {
            if (waitingForPause)
            {
                freezePos = transform.position;
                freezeRotation = transform.rotation;
                if (gameObject.GetComponent<Renderer>() != null)
                    freezeMaterial = gameObject.GetComponent<Renderer>().material;
                if (rb != null && !rb.isKinematic)
                    freezeRbVelocity = rb.velocity;
                if (light != null)
                    freezeLightState = light.enabled;
                if (shakingPlatform != null)
                {
                    freezeShakingPlatformState = shakingPlatform.currentState;
                    freezeShakingPlatformShakeTimer = shakingPlatform.shakeTimer;
                }
                    

                waitingForPause = false;
            }
            transform.SetPositionAndRotation(freezePos, freezeRotation);
            if (gameObject.GetComponent<Renderer>() != null)
                gameObject.GetComponent<Renderer>().material = freezeMaterial;
            if (rb != null && !rb.isKinematic)
                rb.velocity = freezeRbVelocity;
            if (light != null)
                light.enabled = freezeLightState;
            if (shakingPlatform != null)
            {
                shakingPlatform.currentState = freezeShakingPlatformState;
                shakingPlatform.shakeTimer = freezeShakingPlatformShakeTimer;
            }
                
        }
        else
        {
            waitingForPause = true;
        }

        if (timeControl.pauseOn)
            ;
        else if (!timeControl.rewindOn)
        {
            positionHistory.Push(transform.position);
            rotationHistory.Push(transform.rotation);
            if (gameObject.GetComponent<Renderer>() != null)
                materialHistory.Push(gameObject.GetComponent<Renderer>().material);
            if (rb != null && !rb.isKinematic)
                rbVelocityHistory.Push(rb.velocity);
            if (light != null)
                lightStateHistory.Push(light.enabled);
            if (shakingPlatform != null)
            {
                shakingPlatformStateHistory.Push(shakingPlatform.currentState);
                shakingPlatformShakeTimerHistory.Push(shakingPlatform.shakeTimer);
            }
                
        }
        else if (timeControl.rewindOn)
        {
            if (positionHistory.Count() == 0)
            {
                timeControl.rewindOn = false;
            }
            else
            {
                transform.SetPositionAndRotation(positionHistory.Pop(), rotationHistory.Pop());
                if (gameObject.GetComponent<Renderer>() != null)
                    gameObject.GetComponent<Renderer>().material = materialHistory.Pop();
                if (rb != null && !rb.isKinematic)
                    rb.velocity = rbVelocityHistory.Pop();
                if (light != null)
                    light.enabled = lightStateHistory.Pop();
                if (shakingPlatform != null)
                {
                    shakingPlatform.currentState = shakingPlatformStateHistory.Pop();
                    shakingPlatform.shakeTimer = shakingPlatformShakeTimerHistory.Pop();
                }
                    
            }
        }
    }

    public int PeekShakingPlatformState()
    {
        return shakingPlatformStateHistory.Peek();
    }

    public Vector3 PeekPosition()
    {
        return positionHistory.Peek();
    }
}
