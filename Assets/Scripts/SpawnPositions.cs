using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositions : MonoBehaviour
{
    [SerializeField]
    private Vector3 player1Position;
    [SerializeField]
    private Vector3 player2Position;

    [SerializeField]
    private Quaternion player1Rotation;
    [SerializeField]
    private Quaternion player2Rotation;


    public Vector3 GetPosition(ulong clientid)
    {
        if (clientid == 0)
        {
            return player1Position;
        }
        else if (clientid == 1)
        {
            return player2Position;
        }
        return Vector3.zero;
    }

    public Quaternion GetRotation(ulong clientid)
    {
        if (clientid == 0)
        {
            return player1Rotation;
        }
        else if (clientid == 1)
        {
            return player2Rotation;
        }
        return Quaternion.identity;
    }

}
