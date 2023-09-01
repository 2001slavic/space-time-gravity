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
            gravity.scale = 25;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
