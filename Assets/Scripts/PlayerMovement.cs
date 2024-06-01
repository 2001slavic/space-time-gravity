using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
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
    private BuoyantForce buoyantForce;

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
    [HideInInspector]
    public Vector3 groundNormal;
    public float stepOffset;
    public float jumpForce;

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

        // set transform.up to normal of first touched ground if player fell of gravity panel
        if (isGrounded && !_onGravityPanel && lastOnGravityPanel)
        {
            groundNormal = groundHit.normal;
            transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            lastOnGravityPanel = false;
        }

        return _onGravityPanel;
    }

    private void Jump()
    {   
        if (!isGrounded || isOnGravityPanel || !Input.GetButtonDown("Jump"))
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

    /// <summary>
    /// Returns different types of input as out parameters.
    /// </summary>
    /// <param name="rawInput">Vector3 of input as got from Input.GetAxis()</param>
    /// <param name="normalizedInput">Normalized rawInput</param>
    /// <param name="clampedInput">Clamped input so player strafes and moves backwards slower.</param>
    private void GetInput()
    {
        rawInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        normalizedInput = rawInput.normalized;

        if (rawInput.z < 0 || Input.GetButton("Walk"))
        {
            clampedInput = Vector3.ClampMagnitude(rawInput, 0.5f);
        }
        else
        {
            clampedInput = Vector3.ClampMagnitude(rawInput, 1f);
            clampedInput.x *= 0.75f;
        }
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

        if (Input.GetButton("Jump"))
        {
            addedVelocity += groundNormal * swimUpScale;
        }
        else if (Input.GetButton("SwimDown"))
        {
            addedVelocity += -groundNormal * swimUpScale;
        }

        // Calculate the new velocity but clamp it to swimUpScale
        Vector3 newVelocity = rb.velocity + addedVelocity;
        rb.velocity = Vector3.ClampMagnitude(newVelocity, swimUpScale);
    }

    void JumpOutOfWater()
    {
        if (isGrounded || buoyantForce.inWater || !enteredWater || !Input.GetButton("Jump"))
        {
            return;
        }
        rb.AddForce(0.5f * jumpForce * groundNormal, ForceMode.Impulse);
    }

    private void Outro()
    {
        lastGrounded = isGrounded;
        enteredWater = buoyantForce.inWater;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        timeControl = GetComponent<TimeControl>();
        buoyantForce = GetComponent<BuoyantForce>();
        SizeControl sizeControl = GetComponent<SizeControl>();
        

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
        if (developerMode)
        {
            HandleDeveloperMovement();
            return;
        }

        isGrounded = GroundCheck(out RaycastHit groundHit);
        isOnGravityPanel = HandleGravityPanel(groundHit);

        PlayLandClip(); // cannot or difficult to separate

        JumpOutOfWater();

        Jump();

        GetInput();

        HandleWaterVerticalMovement();

        Vector3 direction = transform.TransformDirection(clampedInput).normalized;

        HandleStepUpStairs(transform.TransformDirection(normalizedInput));
        Move(direction, playerSpeed * clampedInput.magnitude);


        Outro();
    }
}
