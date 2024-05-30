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
    [SerializeField]
    private bool developerMode;
    [SerializeField]
    private readonly float devModeSpeed;

    [SerializeField]
    private float handLength = 1.5f;

    private Vector3 lastCheckpointPosition;
    private Quaternion lastCheckpointRotation;
    private Vector3 lastCheckpointVelocity;
    private bool lastCheckpointPauseState;
    private bool lastCheckpointRewindState;
    private float lastCheckpointEffectTime;
    private Vector3 lastCheckpointGroundNormal;

    [SerializeField]
    private readonly float buyoantForceScale = 24.75f;
    [SerializeField]
    private readonly float swimUpScale = 5;

    [SerializeField]
    private bool lastOnGravityPanel;
    [SerializeField]
    private bool isWalking;
    [SerializeField]
    private bool isRunning;
    [SerializeField]
    private bool inWater;
    [SerializeField]
    private bool enteredWater;

    [SerializeField]
    private bool kill;

    private Rigidbody rb;
    private CapsuleCollider collider;
    public TimeControl timeControl;
    public Camera playerCamera;
    [SerializeField]
    private LayerMask clipCheckIgnore;
    [SerializeField]
    private LayerMask groundCheckIgnore;
    [SerializeField]
    private Canvas deathCanvas;
    [SerializeField]
    private Image deathImage;
    public float playerSpeed;
    [SerializeField]
    private float playerGravityScale;
    [SerializeField]
    private bool isGrounded;
    private bool lastGrounded;
    [SerializeField]
    private bool isOnGravityPanel;
    [SerializeField]
    private Vector3 groundNormal;
    private Animator animator;
    public float stepOffset;
    public float jumpForce;

    [SerializeField]
    private float footStepTime = 0.25f;
    private float curFootStepTime;

    private int deathFadePhase;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip electrocutionAudio;
    [SerializeField]
    private AudioClip[] footstepsClips;
    [SerializeField]
    private AudioClip[] jumpClips;
    [SerializeField]
    private AudioClip[] landClips;
    [SerializeField]
    private AudioClip enterWater;
    [SerializeField]
    private AudioClip exitWater;

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
            AudioSource.PlayClipAtPoint(enterWater, transform.position, PlayerPrefs.GetFloat("volume", 0.5f));
            inWater = true;
        }
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpointPosition = transform.position;
            lastCheckpointRotation = transform.rotation;
            lastCheckpointVelocity = rb.velocity;
            lastCheckpointEffectTime = timeControl.effectRemainingTime;
            lastCheckpointGroundNormal = groundNormal;
            lastCheckpointPauseState = timeControl.pauseOn;
            lastCheckpointRewindState = timeControl.rewindOn;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Collider>().gameObject.layer == LayerMask.NameToLayer("Water") &&
            other.CompareTag("Dangerous") &&
            !timeControl.pauseOn &&
            !timeControl.rewindOn)
        {
            if (!kill)
            {
                AudioSource.PlayClipAtPoint(electrocutionAudio, transform.position, PlayerPrefs.GetFloat("volume", 0.5f));
            }

            kill = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Collider>().gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            AudioSource.PlayClipAtPoint(exitWater, transform.position, PlayerPrefs.GetFloat("volume", 0.5f));
            inWater = false;
        }
    }

    private void HandleDeveloperMovement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
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
        //if (isGrounded)
        //{
        //    Vector3 velocityAlongNormal = Vector3.Project(rb.velocity, groundNormal);
        //    rb.velocity -= velocityAlongNormal;
        //}
        
        if (!isGrounded || !Input.GetButtonDown("Jump"))
        {
            return;
        }
        if (!inWater)
        {
            audioSource.clip = jumpClips[UnityEngine.Random.Range((int)0, jumpClips.Length)];
            audioSource.volume = PlayerPrefs.GetFloat("volume", 0.5f) * 0.1f;
            audioSource.Play();
        }
        rb.AddForce(groundNormal * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private void ApplyGravity(float scale)
    {
        rb.AddForce(-groundNormal * scale, ForceMode.Acceleration);
    }

    /// <summary>
    /// Returns different types of input as out parameters.
    /// </summary>
    /// <param name="rawInput">Vector3 of input as got from Input.GetAxis()</param>
    /// <param name="normalizedInput">Normalized rawInput</param>
    /// <param name="clampedInput">Clamped input so player strafes and moves backwards slower.</param>
    private void GetInput(out Vector3 rawInput, out Vector3 normalizedInput, out Vector3 clampedInput)
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
        float distance = collider.radius * transform.localScale.x + 0.01f;

        Vector3 sphere1 = transform.TransformPoint(collider.center) + transform.up * (collider.height * transform.localScale.x / 2 - collider.radius * transform.localScale.x);
        Vector3 sphere2 = transform.TransformPoint(collider.center) - transform.up * (collider.height * transform.localScale.x / 2 - collider.radius * transform.localScale.x);

        bool facingObstacle = Physics.CapsuleCast(sphere1, sphere2, collider.radius * transform.localScale.x, direction, out RaycastHit hit, distance, ~clipCheckIgnore);
        // player step up on stairs
        if (facingObstacle)
        {
            Vector3 stepSphere = transform.TransformPoint(collider.center) - transform.up * ((collider.height * transform.localScale.x / 2) - (collider.radius * transform.localScale.x) - stepOffset);
            bool cannotStepUp = Physics.CapsuleCast(sphere1, stepSphere, collider.radius * transform.localScale.x, direction, out _, distance, ~clipCheckIgnore);
            if (!cannotStepUp && hit.transform.gameObject.layer == LayerMask.NameToLayer("Stairs"))
            {
                rb.position += direction.normalized * 0.05f + groundNormal * 0.05f;
            }
        }
    }

    // Moves player without affecting in-air velocity
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

        // Apply the force
        rb.velocity += direction * speed;
    }

    private void HandleAnimations(Vector3 normalizedInput, Vector3 clampedInput)
    {
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

    private void HandleDeath()
    {
        if (!kill)
            return;
        deathCanvas.gameObject.SetActive(true);
        if (deathFadePhase == 0)
        {
            Color color = deathImage.color;
            color.a += Time.deltaTime;
            if (color.a >= 1)
            {
                color.a = 1;
                deathFadePhase = 1;
                transform.rotation = lastCheckpointRotation;
            }
            deathImage.color = color;
        }
        else if (deathFadePhase == 1)
        {
            Color color = deathImage.color;
            color.a -= Time.deltaTime;
            if (color.a <= 0)
            {
                color.a = 0;
                deathFadePhase = 2;
            }
            deathImage.color = color;

            transform.position = lastCheckpointPosition;
            rb.velocity = lastCheckpointVelocity;
            timeControl.effectRemainingTime = lastCheckpointEffectTime;
            groundNormal = lastCheckpointGroundNormal;
            timeControl.pauseOn = lastCheckpointPauseState;
            timeControl.rewindOn = lastCheckpointRewindState;
        }
        else if (deathFadePhase == 2)
        {
            deathFadePhase = 0;
            kill = false;
            deathCanvas.gameObject.SetActive(false);
        }
    }

    private void PlayLandClip()
    {
        if (!isGrounded || lastGrounded || inWater)
        {
            return;
        }
        audioSource.clip = landClips[UnityEngine.Random.Range((int)0, landClips.Length)];
        audioSource.volume = PlayerPrefs.GetFloat("volume", 0.5f) * 0.1f;
        audioSource.Play();
    }

    private void HandleInteraction()
    {
        if (!Input.GetButtonDown("Use"))
        {
            return;
        }
        RaycastHit[] handHit = Physics.RaycastAll(playerCamera.transform.position, playerCamera.transform.forward, handLength, ~clipCheckIgnore);
        float minDistance = Mathf.Infinity;
        int minIndex = -1;
        for (int i = 0; i < handHit.Length; i++)
        {
            if (handHit[i].distance < minDistance)
            {
                minDistance = handHit[i].distance;
                minIndex = i;
            }
            else if (handHit[i].distance == minDistance && handHit[i].collider.CompareTag("Button"))
            {
                minDistance = handHit[i].distance;
                minIndex = i;
            }
        }
        if (minIndex != -1)
        {
            Debug.Log(handHit[minIndex].collider.gameObject.name);
        }
        if (minIndex != -1 && handHit[minIndex].collider.CompareTag("Button"))
        {
            handHit[minIndex].collider.gameObject.GetComponent<GameButton>().pressed = true;
        }
    }

    private void HandleWaterVerticalMovement()
    {
        if (!inWater)
        {
            return;
        }
        rb.AddForce(groundNormal * buyoantForceScale, ForceMode.Acceleration);

        if (Input.GetButton("Jump") && !isGrounded)
        {
            rb.velocity = Vector3.zero;
            rb.MovePosition(rb.position + transform.up * swimUpScale * Time.deltaTime);
        }
        else if (Input.GetButton("SwimDown") && !isGrounded)
        {
            rb.MovePosition(rb.position - transform.up * swimUpScale * Time.deltaTime);
        }
    }

    private void PlayFootsepsClip()
    {
        if (!isGrounded || !isRunning || inWater || curFootStepTime > 0)
        {
            return;
        }
        audioSource.clip = footstepsClips[UnityEngine.Random.Range((int)0, footstepsClips.Length)];
        audioSource.volume = PlayerPrefs.GetFloat("volume", 0.5f) * 0.1f;
        audioSource.Play();
        curFootStepTime = footStepTime;
    }

    //void HandleWaterEnterAndExit()
    //{
    //    if (inWater)
    //    {
    //        if (sizeChanged)
    //            enteredWater = false;
    //        if (!enteredWater)
    //        {
    //            rb.AddForce(groundNormal * rb.velocity.magnitude * 0.5f, ForceMode.Impulse);
    //            playerSpeed /= 2;
    //            enteredWater = true;
    //        }
    //    }
    //    else
    //    {
    //        if (enteredWater)
    //        {
    //            if (Input.GetButton("Jump"))
    //            {
    //                rb.AddForce(groundNormal * jumpForce, ForceMode.Impulse);
    //            }
    //            //ResetPlayerStats();
    //        }

    //        enteredWater = false;
    //    }
    //}
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        timeControl = GetComponent<TimeControl>();
        SizeControl sizeControl = GetComponent<SizeControl>();
        sizeControl.ResetPlayerStats();

        developerMode = false;

        deathFadePhase = 0;

        groundNormal = new Vector3(0, 1, 0);

        lastOnGravityPanel = false;
        isRunning = false;
        isWalking = false;
        inWater = false;
        enteredWater = false;
        kill = false;
        lastGrounded = true;
        isGrounded = false;

        playerGravityScale = 30;

        deathCanvas.gameObject.SetActive(false);

        curFootStepTime = footStepTime;

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

        PlayLandClip();

        //HandleWaterEnterAndExit();

        Jump();

        GetInput(out _, out Vector3 normalizedInput, out Vector3 clampedInput);

        HandleWaterVerticalMovement();

        Vector3 direction = transform.TransformDirection(clampedInput).normalized;

        HandleStepUpStairs(direction);
        Move(direction, playerSpeed * clampedInput.magnitude);

        HandleAnimations(normalizedInput, clampedInput);

        HandleDeath();

        HandleInteraction();

        PlayFootsepsClip();

        curFootStepTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        ApplyGravity(playerGravityScale);
    }
}
