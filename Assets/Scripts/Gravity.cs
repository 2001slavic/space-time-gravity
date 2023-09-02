using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 groundNormal;
    public float scale = 25f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(-groundNormal * scale, ForceMode.Acceleration);
    }

    void Update()
    {
        
    }
}
