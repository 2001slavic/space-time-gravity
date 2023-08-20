using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 groundNormal;
    public float scale = 4.9f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.AddForce(-groundNormal * scale, ForceMode.Acceleration);
    }
}
