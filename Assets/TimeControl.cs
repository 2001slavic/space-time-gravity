using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{

    public float globalTimeScale = 1f;

    public bool pauseOn;
    public bool rewindOn;

    public float effectMaxTime = 10f;

    public float effectRemainingTime;

    void Start()
    {
        pauseOn = false;
        rewindOn = false;

        effectRemainingTime = effectMaxTime;

        Application.targetFrameRate = 500;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && effectRemainingTime > 0)
        {
            pauseOn = !pauseOn;
            rewindOn = false;
        }
        else if (Input.GetKeyDown(KeyCode.E) && effectRemainingTime > 0)
        {
            rewindOn = !rewindOn;
            pauseOn = false;
        }

        if (!pauseOn)
            globalTimeScale = 1;

        if (!pauseOn && !rewindOn && effectRemainingTime < effectMaxTime)
        {
            effectRemainingTime += 0.5f * Time.deltaTime;
        }
        else if (pauseOn)
        {
            if (effectRemainingTime <= 0)
            {
                pauseOn = false;
                globalTimeScale = 1;
            }
            globalTimeScale = 0;
            effectRemainingTime -= Time.deltaTime;
        }
        else if (rewindOn)
        {
            if (effectRemainingTime <= 0)
                rewindOn = false;
            effectRemainingTime -= Time.deltaTime * 1.25f;
        }
    }
}
