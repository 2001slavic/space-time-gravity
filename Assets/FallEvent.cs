using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallEvent : MonoBehaviour
{

    public Gravity gravity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gravity.scale = 4.91f;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
