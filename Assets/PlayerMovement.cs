using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float curJump;
    private RaycastHit groundHit;
    private RaycastHit gravityPanelHit;

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

    public SizeControl sizeControl;
    public CharacterController controller;
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
    public Vector2 normalizedInput;
    public Vector2 clampedInput;

    public Animator animator;

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

    void Start()
    {
        playerSpeed = refSpeed;
        playerMaxFallVelocity = refMaxFallVelocity;
        groundCheckDistance = refGroundCheckDistance;
        gravityPanelCheckDistance = refGravityPanelCheckDistance;
        jumpHeight = refJumpHeight;
        ceilCheckHeight = refCeilCheckHeight;


        playerGravityScale = 9.81f;
        curGravity = 0;
        curJump = 0;

        groundNormal = new Vector3(0, 1, 0);
        lastOnGravityPanel = false;
        normalizedInput = new Vector2(0, 0);
        clampedInput = new Vector2(0, 0);
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
        }
        else
        {
            if (lastOnGravityPanel)
            {
                groundNormal = groundHit.normal;
                controller.Move(groundNormal);
            }
            lastOnGravityPanel = false;
        }

        if (sizeControl.sizeChanged && (isGrounded || onGravityPanel))
        {
            controller.Move(groundNormal * 2);
        }

        // gravity
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

        isFalling = false;

        if (curGravity > 0)
            isFalling = true;

        if (curGravity < playerMaxFallVelocity)
        {
            curGravity += playerGravityScale * Time.deltaTime;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector2 tmp = new(x, z);
        normalizedInput = tmp.normalized;

        if (z < 0 || Input.GetKey(KeyCode.LeftShift))
        {
            tmp = Vector2.ClampMagnitude(tmp, 0.5f);
        }
        else
        {
            tmp = Vector2.ClampMagnitude(tmp, 1f);
            tmp.x *= 0.75f;
        }

        clampedInput = tmp;

        isWalking = false;
        isRunning = false;

        if (clampedInput.magnitude > 0 && clampedInput.magnitude <= 0.5)
            isWalking = true;
        else if (clampedInput.magnitude > 0.5)
            isRunning = true;

        if (isFalling)
            animator.SetInteger("currentState", animStates["Falling"]);
        else if (isWalking)
        {
            if (normalizedInput.y >= 0.85)
                animator.SetInteger("currentState", animStates["Walk Forwards"]);
            else if (normalizedInput.y < 0.85 && normalizedInput.y >= 0.35 && normalizedInput.x > 0)
                animator.SetInteger("currentState", animStates["Walk Forwards Right"]);
            else if (normalizedInput.y < 0.35 && normalizedInput.y >= -0.35 && normalizedInput.x > 0)
                animator.SetInteger("currentState", animStates["Strafe Right"]);
            else if (normalizedInput.y < -0.35 && normalizedInput.y >= -0.85 && normalizedInput.x > 0)
                animator.SetInteger("currentState", animStates["Walk Backwards Right"]);
            else if (normalizedInput.y < -0.85)
                animator.SetInteger("currentState", animStates["Walk Backwards"]);
            else if (normalizedInput.y >= -0.85 && normalizedInput.y <= -0.35 && normalizedInput.x < 0)
                animator.SetInteger("currentState", animStates["Walk Backwards Left"]);
            else if (normalizedInput.y > -0.35 && normalizedInput.y <= 0.35 && normalizedInput.x < 0)
                animator.SetInteger("currentState", animStates["Strafe Left"]);
            else if (normalizedInput.y > 0.35 && normalizedInput.y < 0.85 && normalizedInput.x < 0)
                animator.SetInteger("currentState", animStates["Walk Forwards Left"]);
        }
        else if (isRunning)
        {
            if (normalizedInput.y >= 0.85)
                animator.SetInteger("currentState", animStates["Run Forwards"]);
            else if (normalizedInput.y < 0.85 && normalizedInput.y >= 0.35 && normalizedInput.x > 0)
                animator.SetInteger("currentState", animStates["Run Forwards Right"]);
            else if (normalizedInput.y > 0.35 && normalizedInput.y < 0.85 && normalizedInput.x < 0)
                animator.SetInteger("currentState", animStates["Run Forwards Left"]);
            else if (normalizedInput.y > -0.35 && normalizedInput.y <= 0.35 && normalizedInput.x < 0)
                animator.SetInteger("currentState", animStates["Strafe Left"]);
            else if (normalizedInput.y < 0.35 && normalizedInput.y >= -0.35 && normalizedInput.x > 0)
                animator.SetInteger("currentState", animStates["Strafe Right"]);
        }
        else
            animator.SetInteger("currentState", animStates["Idle"]);

        Vector3 move = transform.right * clampedInput.x + transform.forward * clampedInput.y;
        if (curGravity < jumpHeight)
            controller.Move(playerSpeed * Time.deltaTime * move);
        else
        {
            controller.Move(0.5f * playerSpeed * Time.deltaTime * move);
        }
            
    }
}
