using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ScaleUINetwork : NetworkBehaviour
{

    private float lastWidth;
    private float lastHeight;
    void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        lastWidth = 800;
        lastHeight = 600;
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        if (lastWidth != Screen.width && lastHeight != Screen.height)
        {
            GetComponent<CanvasScaler>().referenceResolution.Set(Screen.width, Screen.height);
            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
        
    }
}
