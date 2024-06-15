using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField]
    private Vector3 leftPosition;
    [SerializeField]
    private Vector3 originalPosition;
    [SerializeField]
    private Vector3 rightPosition;

    [SerializeField]
    private TimeControl timeControl;

    private int state;

    [SerializeField]
    private float speed;

    private bool lastRewind;


    private void Awake()
    {
        state = 1;
        lastRewind = false;
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (timeControl.rewindOn && (lastRewind != timeControl.rewindOn))
        {
            state = -state;
        }

        lastRewind = timeControl.rewindOn;

        if (transform.localPosition == leftPosition)
        {
            state = 1;
        }
        else if (transform.localPosition == rightPosition)
        {
            state = -1;
        }

        if (timeControl.pauseOn)
        {
            return;
        }

        if (state == -1)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, leftPosition, speed * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, rightPosition, speed * Time.deltaTime);
        }
    }
}
