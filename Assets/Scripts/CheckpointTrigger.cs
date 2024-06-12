using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        GameObject gameObject = other.gameObject;
        Transform transform = gameObject.transform;
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        Vector3 velocity = gameObject.GetComponent<Rigidbody>().velocity;
        TimeControl timeControl = gameObject.GetComponent<TimeControl>();
        SizeControl sizeControl = gameObject.GetComponent<SizeControl>();
        Vector3 groundNormal = gameObject.GetComponent<PlayerMovement>().groundNormal;

        Checkpoint playerCheckpoint = other.gameObject.GetComponent<PlayerDeath>().lastCheckpoint;

        playerCheckpoint.Position = position;
        playerCheckpoint.Rotation = rotation;
        playerCheckpoint.Velocity = velocity;
        playerCheckpoint.Size = sizeControl.size;
        playerCheckpoint.GroundNormal = groundNormal;
        if (timeControl != null)
        {
            playerCheckpoint.EffectRemainingTime = timeControl.effectRemainingTime;
            playerCheckpoint.PauseOn = timeControl.pauseOn;
            playerCheckpoint.RewindOn = timeControl.rewindOn;
        }
    }
}
