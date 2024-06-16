using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTimeControl : MonoBehaviour
{

    public TimeControl timeControl;

    [SerializeField]
    private ShakingPlatform shakingPlatform;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // check if shakingPlatform is falling or fell
        if (shakingPlatform != null)
        {
            if (shakingPlatform.currentState >= 3)
            {
                timeControl.effectRemainingTime = timeControl.effectMaxTime;
            }
            return;
        }
        timeControl.effectRemainingTime = timeControl.effectMaxTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // check if shakingPlatform is falling or fell
        if (shakingPlatform != null)
        {
            if (shakingPlatform.currentState >= 3)
            {
                timeControl.pauseOn = false;
                timeControl.rewindOn = false;
            }
            return;
        }
        timeControl.pauseOn = false;
        timeControl.rewindOn = false;
    }
}
