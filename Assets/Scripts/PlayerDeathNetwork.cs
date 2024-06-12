using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Timeline;
using Unity.Netcode;

public class PlayerDeathNetwork : NetworkBehaviour
{
    public bool killed;
    [SerializeField]
    private Canvas deathCanvas;
    [SerializeField]
    private Image deathImage;
    private int deathFadePhase;

    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Rigidbody playerRb;
    [SerializeField]
    private TimeControl timeControl;
    [SerializeField]
    private PlayerMovementNetwork playerMovement;
    [SerializeField]
    private SizeControlNetwork sizeControl;

    [HideInInspector]
    public Checkpoint lastCheckpoint;

    [SerializeField]
    private SSPause splitscreenPause;

    void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        killed = false;
        lastCheckpoint = new Checkpoint();
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        if (splitscreenPause != null && splitscreenPause.paused)
        {
            deathCanvas.gameObject.SetActive(false);
        }
        if (!killed)
            return;
        deathCanvas.gameObject.SetActive(true);
        // start black screen fading
        if (deathFadePhase == 0)
        {
            Color color = deathImage.color;
            color.a += Time.deltaTime;
            if (color.a >= 1)
            {
                color.a = 1;
                deathFadePhase = 1;

                playerTransform.rotation = lastCheckpoint.Rotation;
            }
            deathImage.color = color;
        }
        // start unfading screen
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


            playerTransform.position = lastCheckpoint.Position;
            playerRb.velocity = lastCheckpoint.Velocity;
            playerMovement.groundNormal = lastCheckpoint.GroundNormal;
            sizeControl.size = lastCheckpoint.Size;
            sizeControl.ResetPlayerStats();
            if (timeControl != null)
            {
                timeControl.effectRemainingTime = lastCheckpoint.EffectRemainingTime;
                timeControl.pauseOn = lastCheckpoint.PauseOn;
                timeControl.rewindOn = lastCheckpoint.RewindOn;
            }
            
        }
        // back alive
        else if (deathFadePhase == 2)
        {
            deathFadePhase = 0;
            killed = false;
            deathCanvas.gameObject.SetActive(false);
        }
    }
}
