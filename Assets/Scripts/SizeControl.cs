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
        
    }
}
