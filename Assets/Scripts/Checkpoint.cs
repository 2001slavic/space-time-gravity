using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Quaternion rotation;
    private Vector3 position;
    private Vector3 velocity;
    private int size;
    private float effectRemainingTime;
    private Vector3 groundNormal;
    private bool pauseOn;
    private bool rewindOn;

    public Quaternion Rotation { get => rotation; set => rotation = value; }
    public Vector3 Position { get => position; set => position = value; }
    public Vector3 Velocity { get => velocity; set => velocity = value; }
    public float EffectRemainingTime { get => effectRemainingTime; set => effectRemainingTime = value; }
    public Vector3 GroundNormal { get => groundNormal; set => groundNormal = value; }
    public bool PauseOn { get => pauseOn; set => pauseOn = value; }
    public bool RewindOn { get => rewindOn; set => rewindOn = value; }
    public int Size { get => size; set => size = value; }
}
