using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleUI : MonoBehaviour
{

    private float lastWidth;
    private float lastHeight;
    void Start()
    {
        lastWidth = 800;
        lastHeight = 600;
    }

    void Update()
    {
        if (lastWidth != Screen.width && lastHeight != Screen.height)
        {
            GetComponent<CanvasScaler>().referenceResolution.Set(Screen.width, Screen.height);
            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
        
    }
}
