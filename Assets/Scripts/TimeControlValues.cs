using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlValues : MonoBehaviour
{

    public TimeControl timeControl;

    private ProgressBarCircle progressBar;
    void Start()
    {
        progressBar = GetComponent<ProgressBarCircle>();
    }

    void Update()
    {
        progressBar.BarValue = Mathf.FloorToInt(timeControl.effectRemainingTime / timeControl.effectMaxTime * 100);
        if (timeControl.pauseOn)
        {
            progressBar.Title = "Pause";
        }
        else if (timeControl.rewindOn)
        {
            progressBar.Title = "Rewind";
        }
        else
        {
            progressBar.Title = "";
        }
    }
}
