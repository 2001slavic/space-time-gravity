using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindHint : MonoBehaviour
{

    [SerializeField]
    private TimeControlBackground timeControlBackground;
    [SerializeField]
    private ShakingPlatform shakingPlatform;

    private readonly float flickerPeriod = 0.1f;
    private float currentFlickerTime;

    private bool hintOn;


    private void Awake()
    {
        hintOn = false;
        currentFlickerTime = flickerPeriod;
    }

    private void ChangeBackgroundColor()
    {
        if (hintOn)
        {
            timeControlBackground.image.color = timeControlBackground.rewindColor;
        }
        else
        {
            timeControlBackground.image.color = timeControlBackground.neutralColor;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (timeControlBackground.timeControl.rewindOn)
        {
            return;
        }

        timeControlBackground.controlOverride = true;

        // check if shakingPlatform is falling or fell
        if (shakingPlatform != null)
        {
            if (shakingPlatform.currentState >= 3)
            {
                ChangeBackgroundColor();
            }
            return;
        }

        ChangeBackgroundColor();

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        timeControlBackground.controlOverride = false;
    }

    private void Update()
    {
        if (currentFlickerTime <= 0)
        {
            hintOn = !hintOn;
            currentFlickerTime = flickerPeriod;
        }
        currentFlickerTime -= Time.deltaTime;
    }
}
