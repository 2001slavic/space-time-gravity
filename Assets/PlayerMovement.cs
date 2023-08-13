using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float curJump;
    private RaycastHit groundHit;
    private RaycastHit gravityPanelHit;
    private Vector3 move;

    private readonly float refSpeed = 12;
    private readonly float refMaxFallVelocity = 100;
    private readonly float refGroundCheckDistance = 1.16f;
    private readonly float refGravityPanelCheckDistance = 1.106f;
    private readonly float refJumpHeight = 10;
    private readonly float refCeilCheckHeight = 2.6f;

    public bool lastOnGravityPanel;
    public bool isWalking;
    public bool isRunning;
    public bool isFalling;

    public Rigidbody rb;
    public CapsuleCollider collider;
    public SizeControl sizeControl;
    public float playerSpeed;
    public float playerGravityScale;
    public Transform groundCheck;
    public LayerMask groundMask;
    public LayerMask gravityPanelMask;
    public float playerMaxFallVelocity;
    public bool isGrounded;
    public bool onGravityPanel;
    public float groundCheckDistance;
    public float gravityPanelCheckDistance;
    public float jumpHeight;
    public float ceilCheckHeight;
    public Vector3 groundNormal;
    public float curGravity;
    public Vector3 input;
    public Vector3 normalizedInput;
    public Vector3 clampedInput;
    public Animator animator;
    public Transform groundCast;

    private int gravityPanelsEntered;

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


    //private void OnCollisionStay(Collision collision)
    //{
    //    if (lastOnGravityPanel)
    //    {
    //        Debug.Log("last");
    //        groundNormal = collision.contacts[0].normal;
    //        transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
    //        onGravityPanel = false;
    //        lastOnGravityPanel = false;
    //    }

    //    if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Gravity Panel"))
    //    {
    //        groundNormal = collision.contacts[0].normal;
    //        transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
    //    }
    //}
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Gravity Panel"))
    //    {
    //        onGravityPanel = true;
    //        gravityPanelsEntered++;
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision == groundCollision)
    //    {
    //        groundCollision = null;
    //        isGrounded = false;
    //    }

    //    if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Gravity Panel"))
    //    {
    //        gravityPanelsEntered--;
    //        if (gravityPanelsEntered == 0)
    //        {
    //            lastOnGravityPanel = true;
    //        }
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawSphere(collider.transform.position + collider.center + new Vector3(0, collider.radius - (collider.height / 2) + 0.1f, 0), collider.radius); // jos
        //Gizmos.DrawSphere(controller.transform.position + controller.center + new Vector3(0, (controller.height / 2) - controller.radius, 0), controller.radius); // sus

        //Gizmos.DrawSphere(controller.transform.position + controller.center, controller.radius);

        Gizmos.DrawSphere(collider.transform.position + collider.center - transform.up * ((collider.height / 2) - collider.radius), collider.radius);
        Gizmos.DrawSphere(collider.transform.position + collider.center + transform.up * ((collider.height / 2) - collider.radius), collider.radius);

        //Gizmos.DrawSphere(transform.position + transform.up * (collider.radius - (collider.height / 2)), collider.radius);
        //Gizmos.DrawSphere(transform.position + transform.up * (collider.height / 2 - collider.radius), collider.radius);
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = rb.position + transform.TransformDirection(input) * playerSpeed * Time.fixedDeltaTime;

        // prevent clipping through walls
        RaycastHit hit;
        if (Physics.Raycast(rb.position, newPosition - rb.position, out hit, Vector3.Distance(rb.position, newPosition)))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Interactive"))
            {
                newPosition = hit.point - (newPosition - rb.position).normalized * 0.01f;
            }
        }

        rb.MovePosition(newPosition);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        playerSpeed = refSpeed;
        playerMaxFallVelocity = refMaxFallVelocity;
        groundCheckDistance = refGroundCheckDistance;
        gravityPanelCheckDistance = refGravityPanelCheckDistance;
        jumpHeight = refJumpHeight;
        ceilCheckHeight = refCeilCheckHeight;


        playerGravityScale = 4.9f;
        curGravity = 0;
        curJump = 0;
        gravityPanelsEntered = 0;

        groundNormal = new Vector3(0, 1, 0);
        lastOnGravityPanel = false;
        normalizedInput = new Vector3(0, 0);
        clampedInput = new Vector3(0, 0);
        move = Vector3.zero;
        animator = GetComponent<Animator>();
        isRunning = false;
        isWalking = false;
        isFalling = false;
        
    }

    void Update()
    {
        switch (sizeControl.curSize)
        {
            case 0:
                playerSpeed = refSpeed / 2;
                playerMaxFallVelocity = refMaxFallVelocity / 2;
                groundCheckDistance = refGroundCheckDistance / 2;
                gravityPanelCheckDistance = refGravityPanelCheckDistance / 2;
                //jumpHeight = refJumpHeight / 2;
                ceilCheckHeight = refCeilCheckHeight / 2;
                break;
            case 1:
                playerSpeed = refSpeed;
                playerMaxFallVelocity = refMaxFallVelocity;
                groundCheckDistance = refGroundCheckDistance;
                gravityPanelCheckDistance = refGravityPanelCheckDistance;
                jumpHeight = refJumpHeight;
                ceilCheckHeight = refCeilCheckHeight;
                break;
            case 2:
                playerSpeed = refSpeed * 1.5f;
                playerMaxFallVelocity = refMaxFallVelocity * 2;
                groundCheckDistance = refGroundCheckDistance * 2;
                gravityPanelCheckDistance = refGravityPanelCheckDistance * 2;
                //jumpHeight = refJumpHeight * 2;
                ceilCheckHeight = refCeilCheckHeight * 2;
                break;
        }

        isGrounded = Physics.SphereCast(groundCast.position, collider.radius, -groundNormal, out groundHit, 0.2f);
        //Debug.Log(groundCast.position);

        onGravityPanel = false;
        if (isGrounded && groundHit.transform.gameObject.layer == LayerMask.NameToLayer("Gravity Panel"))
        {
            onGravityPanel = true;
            groundNormal = groundHit.normal;
            transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            Debug.Log(transform.forward);
            lastOnGravityPanel = true;
        }

        if (isGrounded && !onGravityPanel && lastOnGravityPanel)
        {
            groundNormal = groundHit.normal;
            transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            lastOnGravityPanel = false;
        }

        //onGravityPanel = Physics.SphereCast(collider.transform.position + collider.center + new Vector3(0, collider.radius - (collider.height / 2) + 0.1f, 0), collider.radius, -groundNormal, out gravityPanelHit, gravityPanelCheckDistance, gravityPanelMask);
        //if (onGravityPanel)
        //{
        //    groundNormal = gravityPanelHit.normal;
        //    transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
        //    lastOnGravityPanel = true;
        //}
        //else
        //{
        //    if (lastOnGravityPanel)
        //    {
        //        groundNormal = groundHit.normal;
        //        transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
        //        controller.Move(groundNormal);
        //    }
        //    lastOnGravityPanel = false;
        //}

        //if (sizeControl.sizeChanged && (isGrounded || onGravityPanel))
        //{
        //    controller.Move(groundNormal * 2);
        //}

        // gravity
        rb.AddForce(-groundNormal * playerGravityScale, ForceMode.Acceleration);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(groundNormal * jumpHeight, ForceMode.Impulse);
        }

        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //move = (transform.forward * input.z + transform.right * input.x).normalized;
        //rb.AddForce(move * playerSpeed, ForceMode.Acceleration);
        //Debug.Log(rb.velocity);

        //input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //normalizedInput = input.normalized;

        //float targetSpeed = playerSpeed;

        //if (input.z < 0 || Input.GetKey(KeyCode.LeftShift))
        //{
        //    targetSpeed /= 2;
        //    clampedInput = Vector3.ClampMagnitude(input, 0.5f);
        //}
        //else
        //{
        //    clampedInput = Vector3.ClampMagnitude(input, 1f);
        //    clampedInput.x *= 0.75f;
        //}

        //Vector3 move = targetSpeed * Time.deltaTime * clampedInput;

        //Vector3 movementPlane = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        //if (movementPlane.magnitude < targetSpeed)
        //{
        //    rb.AddForce(move, ForceMode.Force);
        //}

        //isWalking = false;
        //isRunning = false;

        //if (clampedInput.magnitude > 0 && clampedInput.magnitude <= 0.5)
        //    isWalking = true;
        //else if (clampedInput.magnitude > 0.5)
        //    isRunning = true;

        //if (isFalling)
        //    animator.SetInteger("currentState", animStates["Falling"]);
        //else if (isWalking)
        //{
        //    if (normalizedInput.y >= 0.85)
        //        animator.SetInteger("currentState", animStates["Walk Forwards"]);
        //    else if (normalizedInput.y < 0.85 && normalizedInput.y >= 0.35 && normalizedInput.x > 0)
        //        animator.SetInteger("currentState", animStates["Walk Forwards Right"]);
        //    else if (normalizedInput.y < 0.35 && normalizedInput.y >= -0.35 && normalizedInput.x > 0)
        //        animator.SetInteger("currentState", animStates["Strafe Right"]);
        //    else if (normalizedInput.y < -0.35 && normalizedInput.y >= -0.85 && normalizedInput.x > 0)
        //        animator.SetInteger("currentState", animStates["Walk Backwards Right"]);
        //    else if (normalizedInput.y < -0.85)
        //        animator.SetInteger("currentState", animStates["Walk Backwards"]);
        //    else if (normalizedInput.y >= -0.85 && normalizedInput.y <= -0.35 && normalizedInput.x < 0)
        //        animator.SetInteger("currentState", animStates["Walk Backwards Left"]);
        //    else if (normalizedInput.y > -0.35 && normalizedInput.y <= 0.35 && normalizedInput.x < 0)
        //        animator.SetInteger("currentState", animStates["Strafe Left"]);
        //    else if (normalizedInput.y > 0.35 && normalizedInput.y < 0.85 && normalizedInput.x < 0)
        //        animator.SetInteger("currentState", animStates["Walk Forwards Left"]);
        //}
        //else if (isRunning)
        //{
        //    if (normalizedInput.y >= 0.85)
        //        animator.SetInteger("currentState", animStates["Run Forwards"]);
        //    else if (normalizedInput.y < 0.85 && normalizedInput.y >= 0.35 && normalizedInput.x > 0)
        //        animator.SetInteger("currentState", animStates["Run Forwards Right"]);
        //    else if (normalizedInput.y > 0.35 && normalizedInput.y < 0.85 && normalizedInput.x < 0)
        //        animator.SetInteger("currentState", animStates["Run Forwards Left"]);
        //    else if (normalizedInput.y > -0.35 && normalizedInput.y <= 0.35 && normalizedInput.x < 0)
        //        animator.SetInteger("currentState", animStates["Strafe Left"]);
        //    else if (normalizedInput.y < 0.35 && normalizedInput.y >= -0.35 && normalizedInput.x > 0)
        //        animator.SetInteger("currentState", animStates["Strafe Right"]);
        //}
        //else
        //    animator.SetInteger("currentState", animStates["Idle"]);
    }
}
