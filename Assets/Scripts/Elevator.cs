using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private Vector3 leftPosition;
    [SerializeField]
    private Vector3 originalPosition;
    [SerializeField]
    private Vector3 rightPosition;
    public float speed;

    public static bool moveLeft;
    public static bool moveRight;
    [HideInInspector]
    public Vector3 currentTargetPosition;

    void Start()
    {
        moveLeft = false;
        moveRight = false;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            moveRight = true;
        }
            
        if (Input.GetKey(KeyCode.Q))
        {
            moveLeft = true;
        }
            

        if (moveLeft)
        {
            currentTargetPosition = leftPosition;
        }
        else if (moveRight)
        {
            currentTargetPosition = rightPosition;
        }
        else
        {
            currentTargetPosition = originalPosition;
        }
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentTargetPosition, speed * Time.deltaTime);

        moveRight = false;
        moveLeft = false;
    }
}
