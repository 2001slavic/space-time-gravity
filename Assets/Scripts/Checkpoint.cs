using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Checkpoint
{
    private static Quaternion rotation;
    private static Vector3 position;
    private static Vector3 velocity;
    private static float effectRemainingTime;
    private static Vector3 groundNormal;
    private static bool pauseOn;
    private static bool rewindOn;

    public static Quaternion Rotation { get => rotation; set => rotation = value; }
    public static Vector3 Position { get => position; set => position = value; }
    public static Vector3 Velocity { get => velocity; set => velocity = value; }
    public static float EffectRemainingTime { get => effectRemainingTime; set => effectRemainingTime = value; }
    public static Vector3 GroundNormal { get => groundNormal; set => groundNormal = value; }
    public static bool PauseOn { get => pauseOn; set => pauseOn = value; }
    public static bool RewindOn { get => rewindOn; set => rewindOn = value; }
}
