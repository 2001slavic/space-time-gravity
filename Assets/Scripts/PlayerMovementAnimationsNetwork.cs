using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementAnimationsNetwork : NetworkBehaviour
{

    private PlayerMovementNetwork playerMovement;
    private Animator animator;


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
        if (!IsOwner)
        {
            return;
        }
        playerMovement = GetComponent<PlayerMovementNetwork>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        Vector3 rawInput = playerMovement.rawInput;
        Vector3 normalizedInput = playerMovement.normalizedInput;
        Vector3 clampedInput = playerMovement.clampedInput;

        playerMovement.isWalking = false;
        playerMovement.isRunning = false;

        if (clampedInput.magnitude > 0 && clampedInput.magnitude <= 0.5)
            playerMovement.isWalking = true;
        else if (clampedInput.magnitude > 0.5)
            playerMovement.isRunning = true;

        if (!playerMovement.isGrounded)
            animator.SetInteger("currentState", animStates["Falling"]);
        else if (playerMovement.isWalking)
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
        else if (playerMovement.isRunning)
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
