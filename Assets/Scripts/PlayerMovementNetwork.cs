using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovementNetwork : NetworkBehaviour
{
    public bool developerMode;
    [SerializeField]
    private float devModeSpeed;

    [SerializeField]
    private float swimUpScale;

    [SerializeField]
    private bool lastOnGravityPanel;
    [HideInInspector]
    public bool isWalking;
    [HideInInspector]
    public bool isRunning;
    [SerializeField]
    private bool enteredWater;
    [SerializeField]
    private BuoyantForceNetwork buoyantForce;

    private Rigidbody rb;
    private CapsuleCollider collider;
    public TimeControl timeControl;
    public Camera playerCamera;
    [SerializeField]
    private LayerMask groundCheckIgnore;
    public float playerSpeed;
    public bool isGrounded;
    public bool lastGrounded;
    public bool isOnGravityPanel;
    public Vector3 groundNormal;
    public float stepOffset;
    public float jumpForce;

    [SerializeField]
    private PlayerInput playerInput;

    [HideInInspector]
    public Vector3 rawInput;
    [HideInInspector]
    public Vector3 normalizedInput;
    [HideInInspector]
    public Vector3 clampedInput;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] jumpClips;
    [SerializeField]
    private AudioClip[] landClips;

    [SerializeField]
    private PauseNetwork pauseNetwork;

    private void HandleDeveloperMovement()
    {
        rb.velocity = Vector3.zero;
        Vector3 input = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        rb.MovePosition(rb.position + transform.TransformDirection(input) * devModeSpeed);
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
    }

    private bool GroundCheck(out RaycastHit groundHit)
    {
        Vector3 groundCast = transform.TransformPoint(collider.center) - transform.up * ((collider.height * transform.localScale.x / 2) - (collider.radius * transform.localScale.x) - 0.1f);
        return Physics.SphereCast(groundCast, collider.radius * transform.localScale.x - 0.2f, -groundNormal, out groundHit, 0.4f, ~groundCheckIgnore);
    }

    // chnages groundNormal
    private bool HandleGravityPanel(RaycastHit groundHit)
    {
        bool _onGravityPanel = false;
        if (isGrounded && groundHit.transform.gameObject.layer == LayerMask.NameToLayer("Gravity Panel"))
        {
            _onGravityPanel = true;
            groundNormal = groundHit.normal;
            transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            lastOnGravityPanel = true;
        }

        // set transform.up to normal of first touched ground if player fell off gravity panel
        if (isGrounded && !_onGravityPanel && lastOnGravityPanel)
        {
            groundNormal = groundHit.normal;
            transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            lastOnGravityPanel = false;
        }

        return _onGravityPanel;
    }

    private void HandleStepUpStairs(Vector3 direction)
    {
        Vector3 offset = transform.up * (stepOffset + 0.05f) + collider.radius * transform.localScale.x * direction.normalized;
        Vector3 origin = transform.TransformPoint(collider.center) - transform.up * (collider.height * transform.localScale.x / 2);

        if (Physics.Raycast(origin + offset, -transform.up, out RaycastHit hit, stepOffset, ~groundCheckIgnore) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Stairs"))
        {
            Vector3 velocity = Vector3.zero;
            rb.position = Vector3.SmoothDamp(rb.position, hit.point, ref velocity, 1 / (32 * clampedInput.magnitude));
        }
    }

    // Moves player without affecting jump velocity
    private void Move(Vector3 direction, float speed)
    {
        // Calculate the velocity parallel to the up vector
        Vector3 parallelVelocity = Vector3.Dot(rb.velocity, groundNormal) * groundNormal;

        // Calculate the velocity perpendicular to the up vector
        Vector3 perpendicularVelocity = rb.velocity - parallelVelocity;

        // Clamp the magnitude of the perpendicular velocity
        perpendicularVelocity = Vector3.ClampMagnitude(perpendicularVelocity, speed);

        // Reconstruct the total velocity by adding the clamped perpendicular velocity to the parallel velocity
        rb.velocity = perpendicularVelocity + parallelVelocity;

        if (buoyantForce.inWater)
        {
            speed *= 0.5f;
        }

        // Apply the force
        rb.velocity += direction * speed;
    }

    private void PlayLandClip()
    {
        if (!isGrounded || lastGrounded || buoyantForce.inWater)
        {
            return;
        }
        audioSource.clip = landClips[UnityEngine.Random.Range((int)0, landClips.Length)];
        audioSource.volume = PlayerPrefs.GetFloat("volume", 0.5f) * 0.1f;
        audioSource.Play();

    }

    private void HandleWaterVerticalMovement()
    {
        if (!buoyantForce.inWater || isGrounded)
        {
            return;
        }

        Vector3 addedVelocity = Vector3.zero;

        if (playerInput.actions["Jump"].ReadValue<float>() == 1)
        {
            addedVelocity += groundNormal * swimUpScale;
        }
        else if (playerInput.actions["SwimDown"].ReadValue<float>() == 1)
        {
            addedVelocity += -groundNormal * swimUpScale;
        }

        // Calculate the new velocity but clamp it to swimUpScale
        Vector3 newVelocity = rb.velocity + addedVelocity;
        rb.velocity = Vector3.ClampMagnitude(newVelocity, swimUpScale);
    }

    void JumpOutOfWater()
    {
        if (isGrounded || buoyantForce.inWater || !enteredWater || playerInput.actions["Jump"].ReadValue<float>() == 0)
        {
            return;
        }
        rb.AddForce(0.5f * jumpForce * groundNormal, ForceMode.Impulse);
    }

    private void ClampedInputCheck()
    {
        if (rawInput.z < 0 || playerInput.actions["Walk"].ReadValue<float>() == 1)
        {
            clampedInput = Vector3.ClampMagnitude(rawInput, 0.5f);
        }
        else
        {
            clampedInput = Vector3.ClampMagnitude(rawInput, 1f);
            clampedInput.x *= 0.75f;
        }
    }

    private void Outro()
    {
        lastGrounded = isGrounded;
        enteredWater = buoyantForce.inWater;
    }

    private void OnMove(InputValue value)
    {
        if (!IsOwner)
        {
            return;
        }
        if (pauseNetwork.gamePaused)
        {
            return;
        }
        Vector2 vector2value = value.Get<Vector2>();
        rawInput = new Vector3(vector2value.x, 0, vector2value.y);
        normalizedInput = rawInput.normalized;

    }

    private void OnJump()
    {
        if (!IsOwner)
        {
            return;
        }
        if (pauseNetwork.gamePaused)
        {
            return;
        }
        if (!isGrounded || isOnGravityPanel)
        {
            return;
        }
        if (!buoyantForce.inWater)
        {
            audioSource.clip = jumpClips[UnityEngine.Random.Range((int)0, jumpClips.Length)];
            audioSource.volume = PlayerPrefs.GetFloat("volume", 0.5f) * 0.1f;
            audioSource.Play();
        }
        rb.AddForce(groundNormal * jumpForce, ForceMode.Impulse);
    }
    void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        timeControl = GetComponent<TimeControl>();
        buoyantForce = GetComponent<BuoyantForceNetwork>();
        SizeControlNetwork sizeControl = GetComponent<SizeControlNetwork>();


        sizeControl.ResetPlayerStats();

        developerMode = false;
        devModeSpeed = 0.3f;

        groundNormal = new Vector3(0, 1, 0);

        lastOnGravityPanel = false;
        isRunning = false;
        isWalking = false;
        buoyantForce.inWater = false;
        enteredWater = false;
        lastGrounded = true;
        isGrounded = false;

        swimUpScale = 5;

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("volume", 0.5f) * 0.1f;

    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        if (developerMode)
        {
            HandleDeveloperMovement();
            return;
        }

        isGrounded = GroundCheck(out RaycastHit groundHit);

        if (pauseNetwork.gamePaused)
        {
            return;
        }

        isOnGravityPanel = HandleGravityPanel(groundHit);

        ClampedInputCheck();

        PlayLandClip(); // cannot or difficult to separate

        JumpOutOfWater();

        HandleWaterVerticalMovement();

        Vector3 direction = transform.TransformDirection(clampedInput).normalized;

        HandleStepUpStairs(transform.TransformDirection(normalizedInput));
        Move(direction, playerSpeed * clampedInput.magnitude);

        Outro();
    }
}
