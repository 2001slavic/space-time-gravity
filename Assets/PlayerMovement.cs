using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float curJump = 0;
    private RaycastHit groundHit;
    private RaycastHit gravityPanelHit;
    private bool lastOnGravityPanel;

    public CharacterController controller;
    public float playerSpeed = 12;
    public float playerGravityScale = 9.81f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public LayerMask gravityPanelMask;
    public float playerMaxFallVelocity = 10;
    public bool isGrounded;
    public bool onGravityPanel;
    public float groundCheckDistance = 2f;
    public float gravityPanelCheckDistance = 1.9f;
    public float jumpHeight = 10;
    public float ceilCheckHeight = 10;
    public Vector3 groundNormal;
    public float curGravity = 0;

    private void ProcessGravity()
    {

        isGrounded = Physics.Raycast(groundCheck.position, -groundNormal, out groundHit, groundCheckDistance, groundMask);
        if (isGrounded)
        {
            curGravity = 0;
        }

        onGravityPanel = Physics.Raycast(groundCheck.position, -groundNormal, out gravityPanelHit, gravityPanelCheckDistance, gravityPanelMask);

        if (onGravityPanel)
        {
            curGravity = 0;
            curJump = 0;
            groundNormal = gravityPanelHit.normal;
            lastOnGravityPanel = true;
        } else
        {
            if (lastOnGravityPanel)
            {
                groundNormal = groundHit.normal;
            }
            lastOnGravityPanel = false;
        }

        controller.Move(curGravity * Time.deltaTime * -groundNormal);

        // jump
        if (!Physics.Raycast(groundCheck.position, groundCheck.up, ceilCheckHeight))
        {
            controller.Move((curJump - curGravity) * Time.deltaTime * groundCheck.up);
        }
        else
        {
            curJump = 0;
        }

        if (isGrounded && !onGravityPanel)
        {
            curJump = 0;
            if (Input.GetButton("Jump"))
            {
                curJump = jumpHeight;
            }
        }
        if (curGravity < playerMaxFallVelocity)
        {
            curGravity += playerGravityScale * Time.deltaTime;
        }

    }

    private void ProcessMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(playerSpeed * Time.deltaTime * move);

    }

    private void Start()
    {
        groundNormal = new Vector3(0, 1, 0);
        lastOnGravityPanel = false;
    }

    void Update()
    {
        ProcessGravity();
        ProcessMovement();
    }
}
