using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CheckpointTriggerNetwork : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Checkpoint playerCheckpoint = other.gameObject.GetComponent<PlayerDeathNetwork>().lastCheckpoint;
        if (playerCheckpoint == null)
        {
            return;
        }

        GameObject gameObject = other.gameObject;
        Transform transform = gameObject.transform;
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        Vector3 velocity = gameObject.GetComponent<Rigidbody>().velocity;
        SizeControlNetwork sizeControl = gameObject.GetComponent<SizeControlNetwork>();
        Vector3 groundNormal = gameObject.GetComponent<PlayerMovementNetwork>().groundNormal;

        

        playerCheckpoint.Position = position;
        playerCheckpoint.Rotation = rotation;
        playerCheckpoint.Velocity = velocity;
        playerCheckpoint.Size = sizeControl.size;
        playerCheckpoint.GroundNormal = groundNormal;
    }
}
