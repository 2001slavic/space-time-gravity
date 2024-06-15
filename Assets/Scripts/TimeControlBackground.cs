using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeControlBackground : MonoBehaviour
{
    public TimeControl timeControl;
    public Color neutralColor;
    public Color pauseColor;
    public Color rewindColor;

    public Image image;

    public bool controlOverride;
    void Awake()
    {
        controlOverride = false;
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (timeControl.pauseOn)
        {
            image.color = pauseColor;
        }
        else if (timeControl.rewindOn)
        {
            image.color = rewindColor;
        }
        else if (!controlOverride)
        {
            image.color = neutralColor;
        }
    }
}
