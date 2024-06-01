using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyantForce : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]
    private Vector3 groundNormal;
    public float scale;
    [SerializeField]
    private PlayerMovement playerMovement;

    public bool inWater;
    void Start()
    {
        inWater = false;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
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
