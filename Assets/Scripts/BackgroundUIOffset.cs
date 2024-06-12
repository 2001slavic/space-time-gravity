using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackgroundUIOffset : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;

    private readonly Vector3 uiOffset = new Vector3(240, 0, 0);
    void Start()
    {
        if (playerInput.playerIndex == 0)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition3D -= uiOffset;
        }
        else
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition3D += uiOffset;
        }
    }
}
