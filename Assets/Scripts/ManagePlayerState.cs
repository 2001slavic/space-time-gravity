using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ManagePlayerState : MonoBehaviour
{

    public GameObject player;

    public void PlayerSetActive(bool state)
    {
        player.SetActive(state);
    }

    public bool PlayerActiveSelf()
    {
        return player.activeSelf;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
