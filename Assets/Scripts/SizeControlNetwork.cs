using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class SizeControlNetwork : NetworkBehaviour
{
    public int size;
    [SerializeField]
    private CapsuleCollider playerCollider;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private LayerMask sizeChangeMask;
    [SerializeField]
    private PlayerMovementNetwork playerMovement;
    [SerializeField]
    private Rigidbody playerRb;

    [SerializeField]
    private PauseNetwork pauseNetwork;

    private readonly Dictionary<int, float> scales = new()
    {
            { 0, 0.5f },
            { 1, 1f   },
            { 2, 2f   }
    };

    public void ResetPlayerStats()
    {

        switch (size)
        {
            case 0:
                transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                playerRb.mass = 1;
                playerMovement.playerSpeed = 4;
                playerMovement.jumpForce = 10;
                playerMovement.stepOffset = 0.35f;
                break;
            case 1:
                transform.localScale = new Vector3(1, 1, 1);
                playerRb.mass = 2;
                playerMovement.playerSpeed = 8;
                playerMovement.jumpForce = 20;
                playerMovement.stepOffset = 0.7f;
                break;
            case 2:
                transform.localScale = new Vector3(2, 2, 2);
                playerRb.mass = 8;
                playerMovement.playerSpeed = 12;
                playerMovement.stepOffset = 1.4f;
                playerMovement.jumpForce = 80;
                break;
        }
    }

    private int GetIntendedSize()
    {
        if (size == 2)
            return 0;
        else
            return size + 1;
    }

    private bool WillCollide()
    {
        int intendedSize = GetIntendedSize();
        // used to raise the intended collider slightly upper in order to prevent colliding with ground
        Vector3 offset = playerTransform.up * (playerCollider.radius + 0.2f);
        Vector3 upperSphere = transform.TransformPoint(playerCollider.center) + offset + playerTransform.up * (playerCollider.height * scales[intendedSize] / 2 - playerCollider.radius * scales[intendedSize]);
        Vector3 lowerSphere = transform.TransformPoint(playerCollider.center) + offset - playerTransform.up * (playerCollider.height * scales[size] / 2 - playerCollider.radius * scales[size]);

        if (intendedSize > size && Physics.OverlapCapsule(upperSphere, lowerSphere, playerCollider.radius * scales[intendedSize], ~sizeChangeMask).Length > 0)
            return true;
        return false;
    }

    void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        size = 1;
        ResetPlayerStats();
    }

    private void OnSizeChange(InputValue value)
    {
        if (!IsOwner)
        {
            return;
        }
        if (pauseNetwork.gamePaused)
        {
            return;
        }
        if (playerMovement.isOnGravityPanel)
        {
            return;
        }

        if (!WillCollide())
            size = GetIntendedSize();
        // if collision prevents scaling up, change to inferior size
        else if (size > 0)
            size--;
        ResetPlayerStats();
    }

    public bool ChangeToLower()
    {
        if (size <= 0)
        {
            return false;
        }
        size--;
        ResetPlayerStats();
        return true;
    }
}
