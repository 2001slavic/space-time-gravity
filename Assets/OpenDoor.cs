using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public bool isOpen;

    public Transform leftDoorTransform;
    public Transform rightDoorTransform;

    public Vector3 leftDoorOpenDirection;
    public float moveWith = 1.28048f;

    public float delta = 0.01f;

    private Vector3 closedLeftDoorPosition;
    private Vector3 closedRightDoorPosition;
    private Vector3 openLeftDoorPosition;
    private Vector3 openRightDoorPosition;
    void Start()
    {
        isOpen = false;
        closedLeftDoorPosition = leftDoorTransform.position;
        closedRightDoorPosition = rightDoorTransform.position;
        openLeftDoorPosition = closedLeftDoorPosition + leftDoorOpenDirection * moveWith;
        openRightDoorPosition = closedRightDoorPosition - leftDoorOpenDirection * moveWith;
    }

    void Update()
    {
        float diff;
        if (isOpen)
        {
            diff = (leftDoorTransform.position - openLeftDoorPosition).magnitude;
            if (diff >= delta)
                leftDoorTransform.position += leftDoorOpenDirection * Time.deltaTime;

            diff = (rightDoorTransform.position - openRightDoorPosition).magnitude;
            if (diff >= delta)
                rightDoorTransform.position -= leftDoorOpenDirection * Time.deltaTime;
        }
        else
        {
            diff = (leftDoorTransform.position - closedLeftDoorPosition).magnitude;
            if (diff >= delta)
                leftDoorTransform.position -= leftDoorOpenDirection * Time.deltaTime;

            diff = (rightDoorTransform.position - closedRightDoorPosition).magnitude;
            if (diff >= delta)
                rightDoorTransform.position += leftDoorOpenDirection * Time.deltaTime;
        }
    }
}
