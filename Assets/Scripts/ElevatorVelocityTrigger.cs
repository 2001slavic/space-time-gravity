using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorVelocityTrigger : MonoBehaviour
{
    private Vector3 lastPosition;


    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null || rb.isKinematic)
        {
            return;
        }

        Elevator elevator = GetComponentInParent<Elevator>();

        Vector3 positionDifference = elevator.currentTargetPosition - elevator.transform.localPosition;
        rb.position += positionDifference.normalized * elevator.speed * Time.deltaTime;
        if (positionDifference == Vector3.zero)
        {
            Vector3 forceToApply = (elevator.currentTargetPosition - lastPosition).normalized * elevator.speed;
            rb.velocity += forceToApply;
        }
        lastPosition = elevator.transform.localPosition;
    }
}
