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

    private Image image;
    void Start()
    {
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
        else
        {
            image.color = neutralColor;
        }
    }
}
