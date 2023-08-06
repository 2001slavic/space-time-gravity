using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class SizeControl : MonoBehaviour
{
    public Transform player;
    public int curSize;
    public bool sizeChanged;
    void Start()
    {
        curSize = 1;
        sizeChanged = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            sizeChanged = true;
            if (curSize == 2)
                curSize = 0;
            else
                curSize++;
        }
        else
            sizeChanged = false;

        switch (curSize)
        {
            case 0:
                player.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case 1:
                player.transform.localScale = new Vector3(1, 1, 1);
                break;
            case 2:
                player.transform.localScale = new Vector3(2, 2, 2);
                break;
        }
    }
}
