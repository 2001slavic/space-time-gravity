using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTimeControl : MonoBehaviour
{

    public TimeControl timeControl;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timeControl.effectRemainingTime = timeControl.effectMaxTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timeControl.pauseOn = false;
            timeControl.rewindOn = false;
        }
    }
}
