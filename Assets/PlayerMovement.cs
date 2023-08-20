using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public bool developerMode;
    public float devModeSpeed = 10;

    private RaycastHit groundHit;

    private readonly float refSpeed = 12;
    private readonly float refStepOffset = 0.6f;
    public float refMass = 1;
    public float refJumpForce = 10;
    public float archimedesForceScale = 10;
    public float swimUpScale = 10;

    public bool lastOnGravityPanel;
    public bool isWalking;
    public bool isRunning;
    public bool isFalling;
    public bool inWater;

    public Rigidbody rb;
    public CapsuleCollider collider;
    public SizeControl sizeControl;
    public Camera playerCamera;
    public LayerMask clipCheckIgnore;
    public float playerSpeed;
    private float playerGravityScale;
    public bool isGrounded;
    public bool onGravityPanel;
    public Vector3 groundNormal;
    public Vector3 input;
    public Animator animator;
    public float stepOffset;
    public float mass;
    public float jumpForce;

    private readonly Dictionary<string, int> animStates = new()
    {
            { "Idle"                , 0  },
            { "Walk Forwards"       , 1  },
            { "Walk Forwards Right" , 2  },
            { "Strafe Right"        , 3  },
            { "Walk Backwards Right", 4  },
            { "Walk Backwards"      , 5  },
            { "Walk Backwards Left" , 6  },
            { "Strafe Left"         , 7  },
            { "Walk Forwards Left"  , 8  },
            { "Run Forwards Left"   , 9  },
            { "Run Forwards"        , 10 },
            { "Run Forwards Right"  , 11 },
            { "Falling"             , 13 }
        };

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            inWater = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Collider>().gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            inWater = false;
        }
    }

    private void ResetPlayerStats()
    {
        rb.mass = refMass * transform.localScale.x * transform.localScale.x;
        jumpForce = refJumpForce * transform.localScale.x * transform.localScale.x;
        stepOffset = refStepOffset * transform.localScale.x;
        switch (sizeControl.curSize)
        {
            case 0:
                playerSpeed = refSpeed * transform.localScale.x;
                break;
            case 1:
                playerSpeed = refSpeed;
                break;
            case 2:
                playerSpeed = refSpeed * 1.5f;
                break;
        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = transform.TransformDirection(input) * playerSpeed * Time.fixedDeltaTime;
        if (developerMode)
        {
            rb.MovePosition(rb.position + direction);
            return;
        }
        float distance = direction.magnitude;

        Vector3 sphere1 = transform.TransformPoint(collider.center) + transform.up * (collider.height * transform.localScale.x / 2 - collider.radius * transform.localScale.x);
        Vector3 sphere2 = transform.TransformPoint(collider.center) - transform.up * (collider.height * transform.localScale.x / 2 - collider.radius * transform.localScale.x - stepOffset);

        RaycastHit hit;
        // prevent player from clipping through walls
        if (Physics.CapsuleCast(sphere1, sphere2, collider.radius * transform.localScale.x, direction, out hit, distance, ~clipCheckIgnore))
        {
            Vector3 newPosition = rb.position + direction.normalized * hit.distance;
            newPosition += hit.normal * 0.01f; // push away from the wall
            rb.MovePosition(newPosition);
        }
        else
        {
            // If there is no hit, move normally
            rb.MovePosition(rb.position + direction);
        }

        if (inWater && Input.GetButton("Jump"))
        {
            rb.AddForce(transform.up * swimUpScale, ForceMode.VelocityChange);
        }

    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();

        playerSpeed = refSpeed;
        stepOffset = refStepOffset;
        mass = refMass;
        jumpForce = refJumpForce;
        developerMode = false;


        playerGravityScale = 4.9f;

        groundNormal = new Vector3(0, 1, 0);

        lastOnGravityPanel = false;
        isRunning = false;
        isWalking = false;
        isFalling = false;
        inWater = false;
        
    }

    void Update()
    {
        if (developerMode)
        {
            input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0))
            {
                Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit);

                Debug.Log(hit.normal);
            }
            if (Input.GetMouseButtonDown(1))
            {
                Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit);
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                rb.MovePosition(transform.position + transform.up * devModeSpeed);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                rb.MovePosition(transform.position - transform.up * devModeSpeed);
            }
            return;
        }
        if (sizeControl.sizeChanged)
        {
            ResetPlayerStats();
        }

        Vector3 groundCast = transform.TransformPoint(collider.center) - transform.up * ((collider.height * transform.localScale.x / 2) - collider.radius - 0.1f);
        isGrounded = Physics.SphereCast(groundCast, collider.radius, -groundNormal, out groundHit, 0.2f);

        onGravityPanel = false;
        if (isGrounded && groundHit.transform.gameObject.layer == LayerMask.NameToLayer("Gravity Panel"))
        {
            onGravityPanel = true;
            groundNormal = groundHit.normal;
            transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            lastOnGravityPanel = true;
        }

        if (isGrounded && !onGravityPanel && lastOnGravityPanel)
        {
            groundNormal = groundHit.normal;
            transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            lastOnGravityPanel = false;
        }

        // gravity
        rb.AddForce(-groundNormal * playerGravityScale, ForceMode.Acceleration);
        if (inWater)
            rb.AddForce(groundNormal * archimedesForceScale, ForceMode.Force);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(groundNormal * jumpForce, ForceMode.Impulse);
        }

        Vector3 rawInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 normalizedInput = rawInput.normalized;

        Vector3 clampedInput = rawInput;

        if (rawInput.z < 0 || Input.GetKey(KeyCode.LeftShift))
        {
            //targetSpeed /= 2;
            clampedInput = Vector3.ClampMagnitude(rawInput, 0.5f);
        }
        else
        {
            clampedInput = Vector3.ClampMagnitude(rawInput, 1f);
            clampedInput.x *= 0.75f;
        }

        input = clampedInput;

        isWalking = false;
        isRunning = false;

        if (clampedInput.magnitude > 0 && clampedInput.magnitude <= 0.5)
            isWalking = true;
        else if (clampedInput.magnitude > 0.5)
            isRunning = true;

        if (!isGrounded)
            animator.SetInteger("currentState", animStates["Falling"]);
        else if (isWalking)
        {
            if (normalizedInput.z >= 0.85)
                animator.SetInteger("currentState", animStates["Walk Forwards"]);
            else if (normalizedInput.z < 0.85 && normalizedInput.z >= 0.35 && normalizedInput.x > 0)
                animator.SetInteger("currentState", animStates["Walk Forwards Right"]);
            else if (normalizedInput.z < 0.35 && normalizedInput.z >= -0.35 && normalizedInput.x > 0)
                animator.SetInteger("currentState", animStates["Strafe Right"]);
            else if (normalizedInput.z < -0.35 && normalizedInput.z >= -0.85 && normalizedInput.x > 0)
                animator.SetInteger("currentState", animStates["Walk Backwards Right"]);
            else if (normalizedInput.z < -0.85)
                animator.SetInteger("currentState", animStates["Walk Backwards"]);
            else if (normalizedInput.z >= -0.85 && normalizedInput.z <= -0.35 && normalizedInput.x < 0)
                animator.SetInteger("currentState", animStates["Walk Backwards Left"]);
            else if (normalizedInput.z > -0.35 && normalizedInput.z <= 0.35 && normalizedInput.x < 0)
                animator.SetInteger("currentState", animStates["Strafe Left"]);
            else if (normalizedInput.z > 0.35 && normalizedInput.z < 0.85 && normalizedInput.x < 0)
                animator.SetInteger("currentState", animStates["Walk Forwards Left"]);
        }
        else if (isRunning)
        {
            if (normalizedInput.z >= 0.85)
                animator.SetInteger("currentState", animStates["Run Forwards"]);
            else if (normalizedInput.z < 0.85 && normalizedInput.z >= 0.35 && normalizedInput.x > 0)
                animator.SetInteger("currentState", animStates["Run Forwards Right"]);
            else if (normalizedInput.z > 0.35 && normalizedInput.z < 0.85 && normalizedInput.x < 0)
                animator.SetInteger("currentState", animStates["Run Forwards Left"]);
            else if (normalizedInput.z > -0.35 && normalizedInput.z <= 0.35 && normalizedInput.x < 0)
                animator.SetInteger("currentState", animStates["Strafe Left"]);
            else if (normalizedInput.z < 0.35 && normalizedInput.z >= -0.35 && normalizedInput.x > 0)
                animator.SetInteger("currentState", animStates["Strafe Right"]);
        }
        else
            animator.SetInteger("currentState", animStates["Idle"]);
    }
}
