using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Timeline;

public class PlayerDeath : MonoBehaviour
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
    private PlayerMovement playerMovement;

    void Start()
    {
        killed = false;
    }

    void Update()
    {
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

                playerTransform.rotation = Checkpoint.Rotation;
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


            playerTransform.position = Checkpoint.Position;
            playerRb.velocity = Checkpoint.Velocity;
            timeControl.effectRemainingTime = Checkpoint.EffectRemainingTime;
            playerMovement.groundNormal = Checkpoint.GroundNormal;
            timeControl.pauseOn = Checkpoint.PauseOn;
            timeControl.rewindOn = Checkpoint.RewindOn;
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
