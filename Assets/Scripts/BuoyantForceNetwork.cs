using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BuoyantForceNetwork : NetworkBehaviour
{
    private Rigidbody rb;
    public Vector3 groundNormal;
    public float scale;
    [SerializeField]
    private PlayerMovementNetwork playerMovement;

    public bool inWater;
    void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        inWater = false;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        if (!inWater)
        {
            return;
        }
        if (playerMovement != null)
        {
            if (playerMovement.developerMode)
            {
                return;
            }
            groundNormal = playerMovement.groundNormal;
        }
        rb.AddForce(groundNormal * scale, ForceMode.Acceleration);
    }
}
