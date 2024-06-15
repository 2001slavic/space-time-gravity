using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update

    public TimeControl timeControl;
    private int sign;
    private EnumerableDropOutStack<int> signHistory;
    void Start()
    {
        sign = 1;
        signHistory = new EnumerableDropOutStack<int>(Rewindable.HISTORY_STACK_SIZE);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeControl.pauseOn)
            return;

        if (timeControl.rewindOn)
        {
            sign = signHistory.Pop();
            return;
        }
        else
        {
            signHistory.Push(sign);
        }

        transform.Rotate(0, 250 * Time.deltaTime, 0);
        transform.position = transform.position + (new Vector3(Time.deltaTime * sign, 0, 0) * 10);

        if (transform.position.x >= 6 && sign == 1)
        {
            sign = -1;
        }
        else if (transform.position.x <= -6 && sign == -1)
        {
            sign = 1;
        }
    }
}
