using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GravityNetwork : NetworkBehaviour
{
    private Rigidbody rb;
    public Vector3 groundNormal;
    public float scale;
    [SerializeField]
    private PlayerMovementNetwork playerMovement;
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
