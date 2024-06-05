using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplitScreenInit : MonoBehaviour
{
    [SerializeField]
    private Vector3 player1Position;
    [SerializeField]
    private Vector3 player2Position;
    [SerializeField]
    private Quaternion player1Rotation;
    [SerializeField]
    private Quaternion player2Rotation;
    [SerializeField]
    private GameObject playerPrefab;

    void Start()
    {
        if (!SplitScreenMode.isSelected)
        {
            return;
        }

        PlayerInput player1Input = PlayerInput.Instantiate(playerPrefab, pairWithDevice: SplitScreenMode.player1Gamepad);
        GameObject player1 = player1Input.gameObject;
        player1.transform.position = player1Position;
        player1.transform.rotation = player1Rotation;
        player1.transform.SetParent(gameObject.transform);
        PlayerInput player2Input = PlayerInput.Instantiate(playerPrefab, pairWithDevice: SplitScreenMode.player2Gamepad);
        GameObject player2 = player2Input.gameObject;
        player2.transform.position = player2Position;
        player2.transform.rotation = player2Rotation;
        player2.transform.SetParent(gameObject.transform);

        player1.layer = LayerMask.NameToLayer("Player1");
        player2.layer = LayerMask.NameToLayer("Player2");

        GameObject robot1 = GetChildByName.Get(player1, "Robot");
        GameObject robot2 = GetChildByName.Get(player2, "Robot");

        robot1.layer = LayerMask.NameToLayer("Player1");
        robot2.layer = LayerMask.NameToLayer("Player2");

        GameObject player1Camera = GetChildByName.Get(player1, "Player Camera");
        GameObject player2Camera = GetChildByName.Get(player2, "Player Camera");

        player1Camera.GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1);
        player2Camera.GetComponent<Camera>().rect = new Rect(0.5f, 0, 0.5f, 1);
        Destroy(player2Camera.GetComponent<AudioListener>());

        //player1Camera.GetComponent<Camera>().cullingMask &= ~(1 << 9);
        player1Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Player1"));
        player2Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Player2"));

    }

    void Update()
    {
        
    }
}
