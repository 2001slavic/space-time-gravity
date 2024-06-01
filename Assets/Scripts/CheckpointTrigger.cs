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
        float effectRemainingTime = timeControl.effectRemainingTime;
        bool pauseOn = timeControl.pauseOn;
        bool rewindOn = timeControl.rewindOn;
        Vector3 groundNormal = gameObject.GetComponent<PlayerMovement>().groundNormal;

        Checkpoint.Position = position;
        Checkpoint.Rotation = rotation;
        Checkpoint.Velocity = velocity;
        Checkpoint.EffectRemainingTime = effectRemainingTime;
        Checkpoint.PauseOn = pauseOn;
        Checkpoint.RewindOn = rewindOn;
        Checkpoint.GroundNormal = groundNormal;
    }
}
