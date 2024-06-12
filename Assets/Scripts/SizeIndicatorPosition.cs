using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SizeIndicatorPosition : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    void Start()
    {
        if (playerInput.playerIndex == 0)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-89, 171, 0);
        }
        else
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(393, 171, 0);
        }
    }
}
