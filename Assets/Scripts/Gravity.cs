using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 groundNormal;
    public float scale;
    [SerializeField]
    private PlayerMovement playerMovement;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (playerMovement != null)
        {
            if (playerMovement.developerMode)
            {
                return;
            }
            groundNormal = playerMovement.groundNormal;
        }
        rb.AddForce(-groundNormal * scale, ForceMode.Acceleration);
    }
}
